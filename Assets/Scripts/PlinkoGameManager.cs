using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlinkoGameManager : MonoBehaviour
{
    public static PlinkoGameManager Instance { get; private set; }

    [Header("Ball Settings")]
    [SerializeField] private int _ballsAtOnce = 1;
    [SerializeField] private int _maxBallsOnField = 3;

    [Header("Physics Settings")]
    [SerializeField, Range(0f, 1f)] private float _centerBias = 0.02f;
    [SerializeField, Range(0f, 1f)] private float _xDamping = 0.5f;

    [Header("Board Settings")]
    [SerializeField] private int _boardRows = 8;

    [Header("Credits")]
    [SerializeField] private int _startingCredits = 100;
    [SerializeField] private TMP_Text creditLabel;

    [Header("References")]
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private PlinkoBoard board;

    // Live-readable by ball components
    public int BallsAtOnce => _ballsAtOnce;
    public float CenterBias => _centerBias;
    public float XDamping => _xDamping;
    public int BoardRows => _boardRows;

    // Fired when any ball lands: (slotIndex, slotColor)
    public static event Action<int, Color> OnSlotHit;

    // Gradient: edges are red, center is yellow — matches the Python gradient
    static readonly Color ColorEdge = new Color(1f, 0.004f, 0f);
    static readonly Color ColorCenter = new Color(1f, 0.850f, 0f);

    readonly List<GameObject> _activeBalls = new();

    const string kCreditsKey = "plinko_credits";

    int _credits;
    public int Credits => _credits;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _credits = PlayerPrefs.HasKey(kCreditsKey)
            ? PlayerPrefs.GetInt(kCreditsKey)
            : _startingCredits;
        RefreshCreditLabel();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.spaceKey.wasPressedThisFrame) DropBalls();
        if (kb.rKey.wasPressedThisFrame) ResetBoard();
    }

    public void DropBalls()
    {
        for (int i = 0; i < _ballsAtOnce; i++)
        {
            if (_credits <= 0) break;
            if (_activeBalls.Count >= _maxBallsOnField) break;
            _credits--;
            SaveCredits();
            RefreshCreditLabel();
            ballSpawner?.SpawnOne();
        }
    }

    public void ResetBoard()
    {
        PlinkoAnalytics.Instance?.ResetCounts();

        foreach (var b in _activeBalls)
            if (b != null) ballSpawner?.ReturnBall(b);
        _activeBalls.Clear();
    }

    public void ReturnBall(GameObject ball) => ballSpawner?.ReturnBall(ball);

    public void AddCredits(int amount)
    {
        _credits += amount;
        SaveCredits();
        RefreshCreditLabel();
    }

    void SaveCredits() => PlayerPrefs.SetInt(kCreditsKey, _credits);

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(kCreditsKey);
        _credits = _startingCredits;
        RefreshCreditLabel();
    }

    void RefreshCreditLabel()
    {
        if (creditLabel != null)
            creditLabel.text = $"{_credits} credits";
    }

    // Called by BallSpawner so GameManager can track active balls
    public void RegisterBall(GameObject ball) => _activeBalls.Add(ball);

    // Called before a ball is destroyed so it doesn't accumulate as a null ref
    public void UnregisterBall(GameObject ball) => _activeBalls.Remove(ball);

    // Called by PlinkoSlotTrigger when a ball lands
    public void NotifySlotHit(int slotIndex, int totalSlots)
    {
        OnSlotHit?.Invoke(slotIndex, SlotColor(slotIndex, totalSlots));
    }

    // Red (edges) → Yellow (center) gradient, mirrored like the Python version
    public static Color SlotColor(int index, int totalSlots)
    {
        float t = 1f - Mathf.Abs((index / (float)(totalSlots - 1)) * 2f - 1f);
        return Color.Lerp(ColorEdge, ColorCenter, Mathf.Pow(t, 0.4f));
    }

    // --- Called by PlinkoUIController sliders ---

    public void SetBallsAtOnce(float value)
    {
        _ballsAtOnce = Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public void SetCenterBias(float value)
    {
        _centerBias = Mathf.Clamp(value, 0f, 0.5f);
    }

    public void SetXDamping(float value)
    {
        _xDamping = Mathf.Clamp01(value);
    }

    public void SetBoardRows(float value)
    {
        _boardRows = Mathf.Clamp(Mathf.RoundToInt(value), 3, 20);
        if (board != null) board.BoardHeight = _boardRows;
    }
}
