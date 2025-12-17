using UnityEngine;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Text difficultyLabel;

    private GameData gameData;

    void Start()
    {
        // Always ensure gameData is valid
        gameData = GameDataManager.Load();
        if (gameData == null)
        {
            Debug.LogWarning("GameDataManager.Load() returned null, creating defaults.");
            gameData = new GameData();
        }

        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Easy"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Hard"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Extreme"));

            // Set dropdown to saved difficulty
            difficultyDropdown.value = (int)gameData.selectedDifficulty;
            difficultyDropdown.RefreshShownValue();

            // Delay initialization until ScoreManager is ready
            StartCoroutine(InitializeDifficultyDropdown());
        }

        ShowHighScore();
    }

    private IEnumerator InitializeDifficultyDropdown()
    {
        // Wait until ScoreManager.Instance is available
        while (ScoreManager.Instance == null)
            yield return null;

        // Apply saved difficulty to ScoreManager immediately
        DifficultyMode savedMode = (DifficultyMode)difficultyDropdown.value;
        ScoreManager.Instance.SetDifficulty(savedMode);
        UpdateDifficultyLabel(savedMode.ToString(), GetColorForMode(savedMode));

        // Subscribe to dropdown changes
        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);

        // Subscribe to score changes
        ScoreManager.Instance.OnScoreChanged.AddListener(HandleScoreChanged);
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged.RemoveListener(HandleScoreChanged);
    }

    public void ShowHighScore()
    {
        if (ScoreManager.Instance != null && highScoreText != null)
        {
            int hs = ScoreManager.Instance.GetHighScoreForCurrentMode();
            int bestMultiplier = ScoreManager.Instance.GetBestMultiplierForCurrentMode();

            highScoreText.text = "High Score (" + ScoreManager.Instance.currentMode + "): " +
                                 hs.ToString("#,0") +
                                 "\nBest Multiplier: x" + bestMultiplier;
        }
    }

    // --- Combined Reset Button ---
    public void ResetHighScoreAndMultiplierButton()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetHighScoreAndBestMultiplier();
            ShowHighScore();
        }
    }

    private void OnDifficultyChanged(int index)
    {
        if (ScoreManager.Instance == null) return;

        DifficultyMode mode = (DifficultyMode)index;
        ScoreManager.Instance.SetDifficulty(mode);
        UpdateDifficultyLabel(mode.ToString(), GetColorForMode(mode));

        if (gameData == null)
        {
            Debug.LogWarning("GameData was null in OnDifficultyChanged, creating defaults.");
            gameData = new GameData();
        }

        // Save selected difficulty so it persists when returning to menu
        gameData.selectedDifficulty = mode;
        GameDataManager.Save(gameData);

        ShowHighScore();
    }

    private void UpdateDifficultyLabel(string modeName, Color color)
    {
        if (difficultyLabel != null)
        {
            difficultyLabel.text = "Difficulty: " + modeName;
            difficultyLabel.color = color;
        }
    }

    private Color GetColorForMode(DifficultyMode mode)
    {
        switch (mode)
        {
            case DifficultyMode.Easy: return Color.green;
            case DifficultyMode.Hard: return Color.yellow;
            case DifficultyMode.Extreme: return Color.red;
            default: return Color.white;
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        ShowHighScore();
    }
}
