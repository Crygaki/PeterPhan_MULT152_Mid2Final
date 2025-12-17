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

        // Now safe to call
        OnDifficultyChanged(difficultyDropdown.value);
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
            int multiplier = ScoreManager.Instance.GetMultiplierForCurrentMode();

            highScoreText.text = "High Score (" + ScoreManager.Instance.currentMode + "): " +
                                 hs.ToString("#,0") +
                                 "\nBest Multiplier: x" + multiplier;
        }
    }

    // --- Combined Reset Button ---
    public void ResetHighScoreAndMultiplierButton()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetHighScoreAndMultiplier();
            ShowHighScore();
        }
    }

    private void OnDifficultyChanged(int index)
    {
        if (ScoreManager.Instance == null) return;

        DifficultyMode mode = (DifficultyMode)index;

        switch (mode)
        {
            case DifficultyMode.Easy:
                ScoreManager.Instance.SetDifficulty(DifficultyMode.Easy);
                UpdateDifficultyLabel("Easy", Color.green);
                break;
            case DifficultyMode.Hard:
                ScoreManager.Instance.SetDifficulty(DifficultyMode.Hard);
                UpdateDifficultyLabel("Hard", Color.yellow);
                break;
            case DifficultyMode.Extreme:
                ScoreManager.Instance.SetDifficulty(DifficultyMode.Extreme);
                UpdateDifficultyLabel("Extreme", Color.red);
                break;
        }

        if (gameData == null)
        {
            Debug.LogWarning("GameData was null in OnDifficultyChanged, creating defaults.");
            gameData = new GameData();
        }

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

    private void HandleScoreChanged(int newScore)
    {
        ShowHighScore();
    }
}
