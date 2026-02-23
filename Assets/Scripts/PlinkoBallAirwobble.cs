using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlinkoBallAirwobble : MonoBehaviour
{
    [Header("Fallback values (overridden by PlinkoGameManager if present)")]
    public float strength   = 0.05f;
    public float frequency  = 0.8f;
    public float centerBias = 0.02f;
    public float xDamping   = 0.5f;   // multiplies x velocity on each bounce

    Rigidbody2D rb;
    float        tOffset;
    [HideInInspector] public bool claimed;

    void Awake()
    {
        rb      = GetComponent<Rigidbody2D>();
        tOffset = Random.value * 1000f;
    }

    float Bias   => PlinkoGameManager.Instance != null ? PlinkoGameManager.Instance.CenterBias : centerBias;
    float Damping => PlinkoGameManager.Instance != null ? PlinkoGameManager.Instance.XDamping   : xDamping;

    void FixedUpdate()
    {
        // Perlin airwobble — random horizontal drift
        float n = Mathf.PerlinNoise(tOffset, Time.time * frequency);
        float x = (n - 0.5f) * 2f; // [-1..1]
        rb.AddForce(new Vector2(x * strength, 0f));

        // Continuous center bias: pull toward x=0 proportional to offset
        rb.AddForce(new Vector2(-rb.position.x * Bias, 0f));
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // X damping: reduce horizontal velocity so bounce goes more upward
        Vector2 vel = rb.linearVelocity;
        vel.x *= Damping;
        rb.linearVelocity = vel;

        // Impulse center bias on each bounce
        rb.AddForce(new Vector2(-rb.position.x * Bias, 0f), ForceMode2D.Impulse);

        // Flash the peg we hit — no separate trigger collider needed
        col.gameObject.GetComponent<PegFlash>()?.Flash();
    }
}
