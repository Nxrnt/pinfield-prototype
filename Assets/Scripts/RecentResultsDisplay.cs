using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Displays the last N slot hits as color-coded tiles, matching the Python
// "display_last_bins" panel on the right side of the screen.
//
// Setup: create a vertical LayoutGroup UI parent, assign it to `tilesParent`,
// assign a prefab that has Image + TMP_Text children, set `maxResults`.
public class RecentResultsDisplay : MonoBehaviour
{
    [SerializeField] private Transform  tilesParent;
    [SerializeField] private GameObject tilePrefab;   // Image + TMP_Text child
    [SerializeField] private int        maxResults = 4;

    readonly Queue<GameObject> _tiles = new();

    void OnEnable()  => PlinkoGameManager.OnSlotHit += HandleSlotHit;
    void OnDisable() => PlinkoGameManager.OnSlotHit -= HandleSlotHit;

    void HandleSlotHit(int slotIndex, Color color)
    {
        // Trim oldest if at cap
        while (_tiles.Count >= maxResults)
            Destroy(_tiles.Dequeue());

        GameObject tile = Instantiate(tilePrefab, tilesParent);
        tile.transform.SetAsFirstSibling(); // newest on top

        // Background color
        var img = tile.GetComponent<Image>();
        if (img != null) img.color = color;

        // Label text — mirrors Python bin_texts lookup via analytics
        var lbl = tile.GetComponentInChildren<TMP_Text>();
        if (lbl != null) lbl.text = slotIndex.ToString();

        _tiles.Enqueue(tile);
    }

    public void Clear()
    {
        foreach (var t in _tiles)
            if (t != null) Destroy(t);
        _tiles.Clear();
    }
}
