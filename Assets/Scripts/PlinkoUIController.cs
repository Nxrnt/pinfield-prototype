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
        if (dropButton      != null) dropButton.onClick.AddListener(OnDrop);
        if (resetButton     != null) resetButton.onClick.AddListener(OnReset);
        if (allInButton     != null) allInButton.onClick.AddListener(() => PlinkoGameManager.Instance?.BetAllIn());
        if (halfBetButton   != null) halfBetButton.onClick.AddListener(() => PlinkoGameManager.Instance?.BetHalf());
        if (doubleBetButton != null) doubleBetButton.onClick.AddListener(() => PlinkoGameManager.Instance?.BetDouble());

        PlinkoGameManager.OnBetOrCreditsChanged += RefreshDoubleButton;
        RefreshDoubleButton();
    }

    void OnDestroy()
    {
        PlinkoGameManager.OnBetOrCreditsChanged -= RefreshDoubleButton;
    }

    void RefreshDoubleButton()
    {
        var gm = PlinkoGameManager.Instance;
        if (doubleBetButton != null)
            doubleBetButton.interactable = gm != null && gm.BetAmount * 2 <= gm.Credits;
        if (allInButton != null)
            allInButton.interactable = gm != null && gm.Credits > 0;
    }

    void OnDrop() => PlinkoGameManager.Instance?.DropBalls();
    void OnReset() => PlinkoGameManager.Instance?.ResetBoard();
}
