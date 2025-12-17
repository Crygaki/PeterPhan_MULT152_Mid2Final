using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int startingScore = 0;
    public int CurrentScore { get; private set; }

    [Header("Difficulty Settings")]
    public DifficultyMode currentMode = DifficultyMode.Easy;  // uses shared enum

    [Header("UI")]
    public TMP_Text scoreText;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;

    private GameData gameData;

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
        }
    }

    void Start()
    {
        CurrentScore = startingScore;

        // Always ensure gameData is valid
        gameData = GameDataManager.Load();
        if (gameData == null)
        {
            Debug.LogWarning("GameDataManager.Load() returned null, creating defaults.");
            gameData = new GameData();
        }

        currentMode = gameData.selectedDifficulty;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(CurrentScore);
    }

    // --- Score Methods ---
    public void AddScore(int baseAmount)
    {
        float multiplier = GetMultiplierForCurrentMode();
        int amount = Mathf.RoundToInt(baseAmount * multiplier);

        CurrentScore += amount;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(CurrentScore);

        UpdateHighScore(CurrentScore);
        SaveGameData();
    }

    public bool UseScore(int amount)
    {
        if (CurrentScore >= amount)
        {
            CurrentScore -= amount;
            UpdateScoreUI();
            OnScoreChanged?.Invoke(CurrentScore);
            return true;
        }
        return false;
    }

    public void ResetScore()
    {
        CurrentScore = startingScore;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(CurrentScore);
    }

    // --- Difficulty Methods ---
    public void SetDifficulty(DifficultyMode mode)
    {
        currentMode = mode;

        if (gameData == null)
        {
            Debug.LogWarning("GameData was null in SetDifficulty, creating defaults.");
            gameData = new GameData();
        }

        gameData.selectedDifficulty = mode;
        SaveGameData();
        Debug.Log("Difficulty set to: " + mode);
    }

    // --- High Score Methods ---
    public int GetHighScoreForCurrentMode()
    {
        if (gameData == null) return 0;

        return currentMode switch
        {
            DifficultyMode.Easy => gameData.highScoreEasy,
            DifficultyMode.Hard => gameData.highScoreHard,
            DifficultyMode.Extreme => gameData.highScoreExtreme,
            _ => 0
        };
    }

    private void UpdateHighScore(int score)
    {
        if (gameData == null) return;

        switch (currentMode)
        {
            case DifficultyMode.Easy:
                if (score > gameData.highScoreEasy) gameData.highScoreEasy = score;
                break;
            case DifficultyMode.Hard:
                if (score > gameData.highScoreHard) gameData.highScoreHard = score;
                break;
            case DifficultyMode.Extreme:
                if (score > gameData.highScoreExtreme) gameData.highScoreExtreme = score;
                break;
        }
    }

    // --- Multiplier Methods ---
    public int GetMultiplierForCurrentMode()
    {
        if (gameData == null) return 1;

        return currentMode switch
        {
            DifficultyMode.Easy => gameData.multiplierEasy,
            DifficultyMode.Hard => gameData.multiplierHard,
            DifficultyMode.Extreme => gameData.multiplierExtreme,
            _ => 1
        };
    }

    public void UpdateMultiplier(int multiplier)
    {
        if (gameData == null) return;

        switch (currentMode)
        {
            case DifficultyMode.Easy:
                if (multiplier > gameData.multiplierEasy) gameData.multiplierEasy = multiplier;
                break;
            case DifficultyMode.Hard:
                if (multiplier > gameData.multiplierHard) gameData.multiplierHard = multiplier;
                break;
            case DifficultyMode.Extreme:
                if (multiplier > gameData.multiplierExtreme) gameData.multiplierExtreme = multiplier;
                break;
        }
        SaveGameData();
    }

    // --- Combined Reset ---
    public void ResetHighScoreAndMultiplier()
    {
        if (gameData == null) return;

        switch (currentMode)
        {
            case DifficultyMode.Easy:
                gameData.highScoreEasy = 0;
                gameData.multiplierEasy = 0;
                break;
            case DifficultyMode.Hard:
                gameData.highScoreHard = 0;
                gameData.multiplierHard = 0;
                break;
            case DifficultyMode.Extreme:
                gameData.highScoreExtreme = 0;
                gameData.multiplierExtreme = 0;
                break;
        }
        SaveGameData();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";
    }

    private void SaveGameData()
    {
        if (gameData == null)
        {
            Debug.LogWarning("Attempted to save null GameData, creating defaults.");
            gameData = new GameData();
        }
        GameDataManager.Save(gameData);
    }
}
