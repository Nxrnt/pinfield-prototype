using UnityEngine;

// Attach to every peg prefab alongside its SpriteRenderer.
// Add a second CircleCollider2D set to IsTrigger = true with a slightly larger radius
// (e.g. 1.5x the physics collider) so the flash fires when the ball enters the trigger zone.
[RequireComponent(typeof(SpriteRenderer))]
public class PegFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.08f;

    SpriteRenderer sr;
    Color          baseColor;
    float          _flashTimer;

    void Awake()
    {
        sr        = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    public void Flash()
    {
        _flashTimer = flashDuration;
        sr.color    = Color.white;
    }

    void Update()
    {
        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f)
                sr.color = baseColor;
        }
    }
}
