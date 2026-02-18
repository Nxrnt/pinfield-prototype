using UnityEngine;

public class PlinkoSlotTrigger : MonoBehaviour
{
    [SerializeField] private int slotIndex = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Option A: tag your ball prefab as "Ball"
        if (!other.CompareTag("Player")) return;

        PlinkoAnalytics.Instance?.Record(slotIndex);

        // Usually you destroy the ball after it lands
        Destroy(other.gameObject);
    }
}
