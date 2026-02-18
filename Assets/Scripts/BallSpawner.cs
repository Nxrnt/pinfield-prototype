using System.Collections;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 0.01f;

    [Header("Spawn variation (keep small)")]
    [SerializeField] private float xJitter = 0.02f;        // world units
    [SerializeField] private float yJitter = 0.00f;        // usually 0
    [SerializeField] private float xImpulse = 0.15f;       // optional
    [SerializeField] private float torqueImpulse = 0.05f;  // optional
    [SerializeField] private bool useImpulse = true;

    void Start()
    {
        if (spawnPoint == null) spawnPoint = transform;
        StartCoroutine(ballSpawn());
    }

    IEnumerator ballSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            GameObject ball = Instantiate(ballPrefab, RanPoint(), spawnPoint.rotation);

            // Optional: tiny push + spin so identical spawns diverge more naturally
            if (useImpulse)
            {
                var rb = ball.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(new Vector2(Random.Range(-xImpulse, xImpulse), 0f), ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-torqueImpulse, torqueImpulse), ForceMode2D.Impulse);
                }
            }
        }
    }

    Vector3 RanPoint()
    {
        Vector3 p = spawnPoint.position;
        p.x += Random.Range(-xJitter, xJitter);
        p.y += Random.Range(-yJitter, yJitter);
        return p;
    }
}
