using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Wires UI controls to PlinkoGameManager — the Unity equivalent of the
// Python slider panel (Rows, Balls at Once, Center Bias) and Drop button.
//
// Setup (Canvas):
//   - Drop Button    → assign to dropButton
//   - Reset Button   → assign to resetButton
//   - Rows Slider    → assign to rowsSlider       (min 3, max 20)
//   - BallsAtOnce    → assign to ballsSlider      (min 1, max 50)
//   - Center Bias    → assign to centerBiasSlider (min 0, max 0.5)
//   - X Damping      → assign to xDampingSlider   (min 0, max 1)
//   - Optional TMP labels to show live values
public class PlinkoUIController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button dropButton;
    [SerializeField] private Button resetButton;

    [Header("Sliders")]
    [SerializeField] private Slider rowsSlider;
    [SerializeField] private Slider ballsSlider;
    [SerializeField] private Slider centerBiasSlider;
    [SerializeField] private Slider xDampingSlider;

    [Header("Value Labels (optional)")]
    [SerializeField] private TMP_Text rowsLabel;
    [SerializeField] private TMP_Text ballsLabel;
    [SerializeField] private TMP_Text centerBiasLabel;
    [SerializeField] private TMP_Text xDampingLabel;

    void Start()
    {
        if (dropButton  != null) dropButton .onClick.AddListener(OnDrop);
        if (resetButton != null) resetButton.onClick.AddListener(OnReset);

        if (rowsSlider       != null) rowsSlider      .onValueChanged.AddListener(OnRowsChanged);
        if (ballsSlider      != null) ballsSlider     .onValueChanged.AddListener(OnBallsChanged);
        if (centerBiasSlider != null) centerBiasSlider.onValueChanged.AddListener(OnCenterBiasChanged);
        if (xDampingSlider   != null) xDampingSlider  .onValueChanged.AddListener(OnXDampingChanged);

        SyncFromManager();
    }

    // Push current GameManager values into slider positions on start
    void SyncFromManager()
    {
        var gm = PlinkoGameManager.Instance;
        if (gm == null) return;

        if (rowsSlider       != null) rowsSlider      .value = gm.BoardRows;
        if (ballsSlider      != null) ballsSlider     .value = gm.BallsAtOnce;
        if (centerBiasSlider != null) centerBiasSlider.value = gm.CenterBias;
        if (xDampingSlider   != null) xDampingSlider  .value = gm.XDamping;

        UpdateLabels();
    }

    void OnDrop()  => PlinkoGameManager.Instance?.DropBalls();
    void OnReset() => PlinkoGameManager.Instance?.ResetBoard();

    void OnRowsChanged(float v)
    {
        PlinkoGameManager.Instance?.SetBoardRows(v);
        if (rowsLabel != null) rowsLabel.text = $"Rows: {Mathf.RoundToInt(v)}";
    }

    void OnBallsChanged(float v)
    {
        PlinkoGameManager.Instance?.SetBallsAtOnce(v);
        if (ballsLabel != null) ballsLabel.text = $"Balls: {Mathf.RoundToInt(v)}";
    }

    void OnCenterBiasChanged(float v)
    {
        PlinkoGameManager.Instance?.SetCenterBias(v);
        if (centerBiasLabel != null) centerBiasLabel.text = $"Center Bias: {v:0.00}";
    }

    void OnXDampingChanged(float v)
    {
        PlinkoGameManager.Instance?.SetXDamping(v);
        if (xDampingLabel != null) xDampingLabel.text = $"X Damping: {v:0.00}";
    }

    void UpdateLabels()
    {
        if (rowsSlider       != null && rowsLabel       != null) rowsLabel      .text = $"Rows: {Mathf.RoundToInt(rowsSlider.value)}";
        if (ballsSlider      != null && ballsLabel      != null) ballsLabel     .text = $"Balls: {Mathf.RoundToInt(ballsSlider.value)}";
        if (centerBiasSlider != null && centerBiasLabel != null) centerBiasLabel.text = $"Center Bias: {centerBiasSlider.value:0.00}";
        if (xDampingSlider   != null && xDampingLabel   != null) xDampingLabel  .text = $"X Damping: {xDampingSlider.value:0.00}";
    }
}
