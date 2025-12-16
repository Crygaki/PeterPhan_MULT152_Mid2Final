using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Text difficultyLabel;

    private const string DifficultyPrefKey = "SelectedDifficulty";

    void Start()
    {
        if (difficultyDropdown != null)
        {
            // Always clear and repopulate options
            difficultyDropdown.ClearOptions();
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Easy"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Hard"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Extreme"));

            // Load saved difficulty (default Easy if none saved)
            int savedDifficulty = PlayerPrefs.GetInt(DifficultyPrefKey, 0);
            difficultyDropdown.value = savedDifficulty;
            OnDifficultyChanged(savedDifficulty);

            // Listen for changes
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
            ScoreManager.Instance.ResetHighScore(); // resets score + level for current difficulty
            ShowHighScore(); // refresh UI immediately
        }
    }

    private void OnDifficultyChanged(int index)
    {
        if (ScoreManager.Instance == null) return;

        switch (index)
        {
            case 0:
                ScoreManager.Instance.SetDifficulty(DifficultyMode.Easy);
                UpdateDifficultyLabel("Easy", Color.green);
                break;
            case 1:
                ScoreManager.Instance.SetDifficulty(DifficultyMode.Hard);
                UpdateDifficultyLabel("Hard", Color.yellow);
                break;
            case 2:
                ScoreManager.Instance.SetDifficulty(DifficultyMode.Extreme);
                UpdateDifficultyLabel("Extreme", Color.red);
                break;
        }

        PlayerPrefs.SetInt(DifficultyPrefKey, index);
        PlayerPrefs.Save();

        ShowHighScore(); // refresh high score + level for selected difficulty
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
