using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Text difficultyLabel;

    private SettingsData settings;

    void Start()
    {
        settings = SettingsManager.LoadSettings();

        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Easy"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Hard"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Extreme"));

            // Set dropdown value based on enum
            difficultyDropdown.value = (int)settings.selectedDifficulty;
            OnDifficultyChanged(difficultyDropdown.value);

            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }

        ShowHighScore();
    }

    public void ShowHighScore()
    {
        if (ScoreManager.Instance != null && highScoreText != null)
        {
            int hs = ScoreManager.Instance.GetHighScoreForCurrentMode();
            int hl = ScoreManager.Instance.GetHighScoreLevelForCurrentMode();

            highScoreText.text = "High Score (" + ScoreManager.Instance.currentMode + "): " +
                                 hs.ToString("#,0") +
                                 "\nLevel Reached: " + hl;
        }
    }

    public void ResetHighScoreButton()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetHighScore();
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

        // Save enum directly
        settings.selectedDifficulty = mode;
        SettingsManager.SaveSettings(settings);

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
}
