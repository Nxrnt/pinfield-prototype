using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlinkoUIController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button dropButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button allInButton;
    [SerializeField] private Button halfBetButton;
    [SerializeField] private Button doubleBetButton;

    void Start()
    {
        if (dropButton     != null) dropButton.onClick.AddListener(OnDrop);
        if (resetButton    != null) resetButton.onClick.AddListener(OnReset);
        if (allInButton    != null) allInButton.onClick.AddListener(() => PlinkoGameManager.Instance?.BetAllIn());
        if (halfBetButton  != null) halfBetButton.onClick.AddListener(() => PlinkoGameManager.Instance?.BetHalf());
        if (doubleBetButton!= null) doubleBetButton.onClick.AddListener(() => PlinkoGameManager.Instance?.BetDouble());
    }

    // Push current GameManager values into slider positions on start
    void SyncFromManager()
    {
        var gm = PlinkoGameManager.Instance;
        if (gm == null) return;
    }

    void Update()
    {
        if (doubleBetButton == null) return;
        var gm = PlinkoGameManager.Instance;
        doubleBetButton.interactable = gm != null && gm.BetAmount * 2 <= gm.Credits;
    }

    void OnDrop() => PlinkoGameManager.Instance?.DropBalls();
    void OnReset() => PlinkoGameManager.Instance?.ResetBoard();
}
