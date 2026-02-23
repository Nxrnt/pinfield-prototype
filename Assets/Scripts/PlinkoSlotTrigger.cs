using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlinkoSlotTrigger : MonoBehaviour
{
    [SerializeField] private int slotIndex = 0;
    [SerializeField] private int totalSlots = 9;
    [SerializeField] private Transform spritePos;

    [Header("Bounce Animation")]
    [SerializeField] private float bounceDepth = 0.1f;
    [SerializeField] private float bounceDownTime = 0.07f;
    [SerializeField] private float bounceUpTime = 0.14f;

    [Header("Audio")]
    [SerializeField] private AudioClip landClip;
    [SerializeField, Range(0.5f, 3f)] private float pitchMin = 0.7f;
    [SerializeField, Range(0.5f, 3f)] private float pitchMax = 2.0f;
    [SerializeField, Range(0f, 1f)] private float volume = 2.0f;

    static readonly List<float> s_activeMults = new();
    const int kMaxConcurrent = 3;

    Vector3 _restPos;
    Coroutine _bounceRoutine;
    AudioSource _audio;
    float _mult;

    void Awake()
    {
        if (spritePos != null) _restPos = spritePos.localPosition;

        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;
        _audio.volume = 1f;
    }

    // Center anchor → edge anchor, snapped to clean numbers in log-space
    static readonly float[] AestheticMults =
        { 1f, 2f, 3f, 5f, 10f, 20f, 25f, 50f };

    public void Init(int index, int total)
    {
        slotIndex = index;
        totalSlots = total;

        Color c = PlinkoGameManager.SlotColor(slotIndex, totalSlots);
        if (spritePos != null && spritePos.TryGetComponent<SpriteRenderer>(out var sr))
            sr.color = c;

        _mult = SnappedMultiplier(index, total);

        var label = transform.parent.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = _mult < 1f ? $"{_mult}x" : $"{Mathf.RoundToInt(_mult)}x";
    }

    static readonly float[] FixedPrefix = { 0f, 0f, 1f, 2f };

    static float SnappedMultiplier(int slotIndex, int totalSlots)
    {
        float center    = (totalSlots - 1) * 0.5f;
        int   distIndex = Mathf.RoundToInt(Mathf.Abs(slotIndex - center));
        int   maxDist   = Mathf.RoundToInt(center);

        if (distIndex >= maxDist)               return 50f;
        if (distIndex < FixedPrefix.Length)     return FixedPrefix[distIndex];

        // Harsh ease-in ramp from 5x → 50x across remaining slots before the edge
        int   rampStart = FixedPrefix.Length;           // 4
        int   rampCount = maxDist - rampStart;
        if (rampCount <= 0) return 50f;

        // Normalised position [0..1] through the ramp, then power-curved for harsh ease-in
        float t      = (float)(distIndex - rampStart + 1) / (rampCount + 1);
        float curved = Mathf.Pow(t, 2.5f);             // harsh ease-in
        float raw    = 5f * Mathf.Pow(10f, curved);    // 5x → 50x  (10^0=1, 10^1=10, ×5)

        float logRaw = Mathf.Log(raw);
        float best   = AestheticMults[0];
        float bestD  = float.MaxValue;
        foreach (float v in AestheticMults)
        {
            float d = Mathf.Abs(logRaw - Mathf.Log(v));
            if (d < bestD) { bestD = d; best = v; }
        }
        return best;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var wobble = other.GetComponent<PlinkoBallAirwobble>();
        if (wobble == null || wobble.claimed) return;
        wobble.claimed = true;

        PlinkoAnalytics.Instance?.Record(slotIndex);
        PlinkoGameManager.Instance?.NotifySlotHit(slotIndex, totalSlots);
        PlinkoGameManager.Instance?.UnregisterBall(other.gameObject);
        int bet = PlinkoGameManager.Instance?.BetAmount ?? 1;
        PlinkoGameManager.Instance?.AddCredits(Mathf.RoundToInt(_mult * bet));

        if (landClip != null)
        {
            bool atCap = s_activeMults.Count >= kMaxConcurrent;
            float lowestActive = atCap ? LowestActiveMult() : 0f;

            if (!atCap || _mult > lowestActive)
            {
                // At cap: displace the lowest-priority slot (its coroutine will clean up harmlessly)
                if (atCap) s_activeMults.Remove(lowestActive);

                float capturedMult = _mult;
                s_activeMults.Add(capturedMult);

                float t = Mathf.InverseLerp(Mathf.Log(0.25f), Mathf.Log(50f), Mathf.Log(_mult));
                _audio.pitch = Mathf.Lerp(pitchMin, pitchMax, t);
                _audio.PlayOneShot(landClip, volume / s_activeMults.Count);
                StartCoroutine(DecrementAfterClip(landClip.length / _audio.pitch, capturedMult));
            }
        }

        if (_bounceRoutine != null) StopCoroutine(_bounceRoutine);
        _bounceRoutine = StartCoroutine(SpritePosBounce());
        PlinkoGameManager.Instance?.ReturnBall(other.gameObject);
    }

    static float LowestActiveMult()
    {
        float min = float.MaxValue;
        foreach (float v in s_activeMults) if (v < min) min = v;
        return min;
    }

    private IEnumerator DecrementAfterClip(float duration, float mult)
    {
        yield return new WaitForSeconds(duration);
        s_activeMults.Remove(mult); // no-op if already displaced by a higher-priority sound
    }

    private IEnumerator SpritePosBounce()
    {
        // Always lerp from wherever we currently are down to pressed,
        // then back to the fixed rest position — safe for rapid re-triggers.
        Vector3 fromPos = spritePos.localPosition;
        Vector3 pressed = _restPos + new Vector3(0f, -bounceDepth, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bounceDownTime;
            spritePos.localPosition = Vector3.Lerp(fromPos, pressed, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bounceUpTime;
            spritePos.localPosition = Vector3.Lerp(pressed, _restPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        spritePos.localPosition = _restPos;
        _bounceRoutine = null;
    }
}
