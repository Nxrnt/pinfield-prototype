using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlinkoBoard : MonoBehaviour
{
    [SerializeField] private int _boardHeight = 6;
    [SerializeField] private int _bucketCount = 7;
    [SerializeField] private float _pegSpacing = 1f;
    [SerializeField] private float _rowSpacing = 1f;

    [SerializeField] private GameObject plinkoPeg;
    [SerializeField] private GameObject plinkoBucket;

    public int BoardHeight
    {
        get => _boardHeight;
        set { _boardHeight = value; GenerateBoard(); }
    }

    public int BucketCount
    {
        get => _bucketCount;
        set { _bucketCount = value; GenerateBoard(); }
    }

    public float PegSpacing
    {
        get => _pegSpacing;
        set { _pegSpacing = value; GenerateBoard(); }
    }

    public float RowSpacing
    {
        get => _rowSpacing;
        set { _rowSpacing = value; GenerateBoard(); }
    }

    void Start()
    {
        GenerateBoard();
    }

    // Fires in the editor whenever a serialized field is changed in the Inspector.
    void OnValidate()
    {
#if UNITY_EDITOR
        // Defer so Instantiate/DestroyImmediate run outside the serialization call stack.
        EditorApplication.delayCall += () => { if (this != null) GenerateBoard(); };
#else
        if (Application.isPlaying) GenerateBoard();
#endif
    }

    public void GenerateBoard()
    {
        ClearBoard();
        Debug.Log("Generating Board...");

        // Triangle: row 0 (top) has 3 peg, row i has i+3 pegs.
        for (int row = 0; row < _boardHeight; row++)
        {
            int pegsInRow = row + 3;
            float startX = -(pegsInRow - 1) * _pegSpacing / 2f;
            float y = -row * _rowSpacing;

            for (int col = 0; col < pegsInRow; col++)
            {
                float x = startX + col * _pegSpacing;
                SpawnPrefab(plinkoPeg, new Vector3(x, y, 0f));
            }
        }

        // Buckets sit one row below the last peg row, centered independently.
        float bucketY = -_boardHeight * _rowSpacing;
        float bucketStartX = -(_bucketCount - 1) * _pegSpacing / 2f;

        for (int i = 0; i < _bucketCount; i++)
        {
            float x = bucketStartX + i * _pegSpacing;
            var go = Instantiate(plinkoBucket, transform.position + new Vector3(x, bucketY, 0f), Quaternion.identity, transform);
            var trigger = go.GetComponentInChildren<PlinkoSlotTrigger>();
            if (trigger != null) trigger.Init(i, _bucketCount);
        }
    }

    void ClearBoard()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    void SpawnPrefab(GameObject prefab, Vector3 localOffset)
    {
        if (prefab == null) return;
        Instantiate(prefab, transform.position + localOffset, Quaternion.identity, transform);
    }
}