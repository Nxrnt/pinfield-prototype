using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlinkoAnalytics : MonoBehaviour
{
    public static PlinkoAnalytics Instance { get; private set; }

    [Header("Your setup")]
    [SerializeField] private int pegRows = 8;   // fixed by you
    [SerializeField] private int slotCount = 9; // fixed by you

    private int[] counts;
    private int total;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        counts = new int[slotCount];
    }

    public void Record(int slotIndex)
    {
        if ((uint)slotIndex >= (uint)counts.Length) return;
        counts[slotIndex]++;
        total++;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.rKey.wasPressedThisFrame) ResetCounts();
        if (kb.pKey.wasPressedThisFrame) PrintReport();
    }

    void ResetCounts()
    {
        Array.Clear(counts, 0, counts.Length);
        total = 0;
        Debug.Log("Plinko analytics reset.");
    }

    void PrintReport()
    {
        if (total == 0) { Debug.Log("No balls recorded yet."); return; }

        // Ideal weights for n=8 are C(8,k): 1,8,28,56,70,56,28,8,1
        // We compute them generally so it still works if you change rows later.
        double chi = 0.0;

        string msg = $"Plinko report | rows={pegRows} slots={slotCount} total={total}\n";
        for (int k = 0; k < slotCount; k++)
        {
            double p = BinomialProb(pegRows, k);
            double expected = p * total;
            double observed = counts[k];

            double pct = observed * 100.0 / total;
            double idealPct = p * 100.0;

            // chi-square contribution
            if (expected > 1e-9)
            {
                double diff = observed - expected;
                chi += (diff * diff) / expected;
            }

            msg += $"slot {k}: {counts[k],5}  ({pct,6:0.00}%)  ideal {idealPct,6:0.00}%\n";
        }

        double mean = 0.0;
        for (int k = 0; k < slotCount; k++)
            mean += k * counts[k];
        mean /= total;

        double var = 0.0;
        for (int k = 0; k < slotCount; k++)
        {
            double d = k - mean;
            var += d * d * counts[k];
        }
        var /= total;

        Debug.Log($"Mean slot: {mean:0.###} (ideal 4.0) | Variance: {var:0.###} (ideal 2.0)");
        msg += $"Chi-square: {chi:0.###} (lower = closer to ideal)\n";
        Debug.Log(msg);
    }

    static double BinomialProb(int n, int k)
    {
        if (k < 0 || k > n) return 0.0;
        return Combination(n, k) * Math.Pow(0.5, n);
    }

    static double Combination(int n, int k)
    {
        k = Math.Min(k, n - k);
        double c = 1.0;
        for (int i = 1; i <= k; i++)
        {
            c *= (n - (k - i));
            c /= i;
        }
        return c;
    }
}
