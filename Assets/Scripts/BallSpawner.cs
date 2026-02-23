using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform  spawnPoint;

    [Header("Auto-spawn (optional)")]
    [SerializeField] private bool  autoSpawn  = false;
    [SerializeField] private float spawnDelay = 0.5f;

    [Header("Spawn variation (keep small)")]
    [SerializeField] private float xJitter       = 0.02f;
    [SerializeField] private float yJitter       = 0.00f;
    [SerializeField] private float xImpulse      = 0.15f;
    [SerializeField] private float torqueImpulse = 0.05f;
    [SerializeField] private bool  useImpulse    = true;

    ObjectPool<GameObject> _pool;

    void Awake()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc:      () => Instantiate(ballPrefab),
            actionOnGet:     OnGetBall,
            actionOnRelease: go => go.SetActive(false),
            actionOnDestroy: go => Destroy(go),
            collectionCheck: false,
            defaultCapacity: 16,
            maxSize:         64
        );
    }

    void Start()
    {
        if (spawnPoint == null) spawnPoint = transform;
        if (autoSpawn) StartCoroutine(AutoSpawnLoop());
    }

    static void OnGetBall(GameObject go)
    {
        go.SetActive(true);
        if (go.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity  = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        if (go.TryGetComponent<Collider2D>(out var col))
            col.enabled = true;
    }

    IEnumerator AutoSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            PlinkoGameManager.Instance?.DropBalls();
        }
    }

    // Called directly by PlinkoGameManager (Space key / Drop button)
    public void SpawnOne()
    {
        if (ballPrefab == null) return;

        GameObject ball = _pool.Get();
        ball.transform.SetPositionAndRotation(RanPoint(), spawnPoint.rotation);
        PlinkoGameManager.Instance?.RegisterBall(ball);

        if (useImpulse && ball.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce(new Vector2(Random.Range(-xImpulse, xImpulse), 0f), ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torqueImpulse, torqueImpulse), ForceMode2D.Impulse);
        }
    }

    public void ReturnBall(GameObject ball) => _pool.Release(ball);

    Vector3 RanPoint()
    {
        Vector3 p = spawnPoint.position;
        p.x += Random.Range(-xJitter, xJitter);
        p.y += Random.Range(-yJitter, yJitter);
        return p;
    }
}
