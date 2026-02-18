using UnityEngine;

public class AirWobble2D : MonoBehaviour
{
    public float strength = 0.05f;   // small
    public float frequency = 0.8f;

    Rigidbody2D rb;
    float tOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tOffset = Random.value * 1000f; // per-ball uniqueness
    }

    void FixedUpdate()
    {
        float n = Mathf.PerlinNoise(tOffset, Time.time * frequency);
        float x = (n - 0.5f) * 2f; // [-1..1]
        rb.AddForce(new Vector2(x * strength, 0f));
    }
}
