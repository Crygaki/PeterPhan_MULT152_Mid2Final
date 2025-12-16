using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Difficulty Settings")]
    public DifficultyMode currentMode = DifficultyMode.Easy;

    [SerializeField] private int baseThreshold;
    [SerializeField] private float growthFactor;
    [SerializeField] private int appleTreeSpeedIncrease;
    [SerializeField] private float appleDropDelayMultiplier;

    public int score { get; private set; }
    public int level { get; private set; } = 1;
    public int sessionTotalScore { get; private set; } = 0;

    // High scores + level reached for each difficulty
    private int highScoreEasy, highScoreHard, highScoreExtreme;
    private int highScoreLevelEasy, highScoreLevelHard, highScoreLevelExtreme;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved high scores and levels
        highScoreEasy = PlayerPrefs.GetInt("HighScore_Easy", 0);
        highScoreHard = PlayerPrefs.GetInt("HighScore_Hard", 0);
        highScoreExtreme = PlayerPrefs.GetInt("HighScore_Extreme", 0);

        highScoreLevelEasy = PlayerPrefs.GetInt("HighScoreLevel_Easy", 1);
        highScoreLevelHard = PlayerPrefs.GetInt("HighScoreLevel_Hard", 1);
        highScoreLevelExtreme = PlayerPrefs.GetInt("HighScoreLevel_Extreme", 1);

        ApplyDifficultySettings();
    }

    public void SetDifficulty(DifficultyMode mode)
    {
        currentMode = mode;
        ApplyDifficultySettings();
    }

    private void ApplyDifficultySettings()
    {
        switch (currentMode)
        {
            case DifficultyMode.Easy:
                baseThreshold = 1000;
                growthFactor = 1.3f;
                appleTreeSpeedIncrease = 1;
                appleDropDelayMultiplier = 0.9f;
                break;
            case DifficultyMode.Hard:
                baseThreshold = 1500;
                growthFactor = 1.5f;
                appleTreeSpeedIncrease = 2;
                appleDropDelayMultiplier = 0.8f;
                break;
            case DifficultyMode.Extreme:
                baseThreshold = 2000;
                growthFactor = 2.0f;
                appleTreeSpeedIncrease = 3;
                appleDropDelayMultiplier = 0.7f;
                break;
        }
    }

    public void AddPoints(int points)
    {
        score += points;
        if (points > 0) sessionTotalScore += points;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (sessionTotalScore >= GetScoreThreshold(level + 1))
        {
            level++;
            OnLevelUp();
        }
    }

    private int GetScoreThreshold(int targetLevel)
    {
        if (targetLevel == 1) return 0;
        return (int)(baseThreshold * Mathf.Pow(growthFactor, targetLevel - 2));
    }

    private void OnLevelUp()
    {
        Debug.Log("Level Up! Current Level: " + level);

        // --- Play Level Up SFX ---
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUpSfx();
        }

        AppleTree tree = FindFirstObjectByType<AppleTree>();
        if (tree != null)
        {
            tree.appleDropDelay *= appleDropDelayMultiplier;
            tree.speed += appleTreeSpeedIncrease;
        }
    }

    public void ResetScore()
    {
        score = 0;
        level = 1;
        sessionTotalScore = 0;
    }

    public void ResetHighScore()
    {
        switch (currentMode)
        {
            case DifficultyMode.Easy:
                highScoreEasy = 0;
                highScoreLevelEasy = 1;
                PlayerPrefs.SetInt("HighScore_Easy", 0);
                PlayerPrefs.SetInt("HighScoreLevel_Easy", 1);
                break;
            case DifficultyMode.Hard:
                highScoreHard = 0;
                highScoreLevelHard = 1;
                PlayerPrefs.SetInt("HighScore_Hard", 0);
                PlayerPrefs.SetInt("HighScoreLevel_Hard", 1);
                break;
            case DifficultyMode.Extreme:
                highScoreExtreme = 0;
                highScoreLevelExtreme = 1;
                PlayerPrefs.SetInt("HighScore_Extreme", 0);
                PlayerPrefs.SetInt("HighScoreLevel_Extreme", 1);
                break;
        }
        PlayerPrefs.Save();
    }

    // Getters
    public int GetHighScoreForCurrentMode()
    {
        switch (currentMode)
        {
            case DifficultyMode.Easy: return highScoreEasy;
            case DifficultyMode.Hard: return highScoreHard;
            case DifficultyMode.Extreme: return highScoreExtreme;
            default: return 0;
        }
    }

    public int GetHighScoreLevelForCurrentMode()
    {
        switch (currentMode)
        {
            case DifficultyMode.Easy: return highScoreLevelEasy;
            case DifficultyMode.Hard: return highScoreLevelHard;
            case DifficultyMode.Extreme: return highScoreLevelExtreme;
            default: return 1;
        }
    }

    public void ReloadScene()
    {
        // Compare before updating
        int hs = GetHighScoreForCurrentMode();
        int hl = GetHighScoreLevelForCurrentMode();

        bool isNewScore = score > hs;
        bool isNewLevel = (score == hs && level > hl);

        PlayerPrefs.SetInt("IsNewScore", isNewScore ? 1 : 0);
        PlayerPrefs.SetInt("IsNewLevel", isNewLevel ? 1 : 0);

        // Update high score if beaten
        if (isNewScore || isNewLevel)
        {
            switch (currentMode)
            {
                case DifficultyMode.Easy:
                    highScoreEasy = score;
                    highScoreLevelEasy = level;
                    PlayerPrefs.SetInt("HighScore_Easy", highScoreEasy);
                    PlayerPrefs.SetInt("HighScoreLevel_Easy", highScoreLevelEasy);
                    break;
                case DifficultyMode.Hard:
                    highScoreHard = score;
                    highScoreLevelHard = level;
                    PlayerPrefs.SetInt("HighScore_Hard", highScoreHard);
                    PlayerPrefs.SetInt("HighScoreLevel_Hard", highScoreLevelHard);
                    break;
                case DifficultyMode.Extreme:
                    highScoreExtreme = score;
                    highScoreLevelExtreme = level;
                    PlayerPrefs.SetInt("HighScore_Extreme", highScoreExtreme);
                    PlayerPrefs.SetInt("HighScoreLevel_Extreme", highScoreLevelExtreme);
                    break;
            }
        }

        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("FinalLevel", level);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameOverScene");
    }
}
