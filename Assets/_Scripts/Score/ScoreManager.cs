using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.IO; // for File IO

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int startingScore = 0;
    public int CurrentScore { get; private set; }

    // Scene-only pool for RMB cooldown
    public int RmbCooldownScore { get; private set; }

    [Header("Difficulty Settings")]
    public DifficultyMode currentMode = DifficultyMode.Easy;

    [Header("UI")]
    public TMP_Text scoreText;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;

    private GameData gameData;
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CurrentScore = startingScore;
        RmbCooldownScore = startingScore;

        if (gameData == null)
            gameData = new GameData();

        ApplyDifficultyFromData();

        UpdateScoreUI();
        OnScoreChanged?.Invoke(CurrentScore);
    }

    private void ApplyDifficultyFromData()
    {
        if (gameData != null)
        {
            currentMode = gameData.selectedDifficulty;
            Debug.Log("Difficulty applied from GameData: " + currentMode);
        }
        else
        {
            currentMode = DifficultyMode.Easy;
            Debug.LogWarning("GameData was null, defaulting to Easy.");
        }
    }

    // --- Score Methods ---
    public void AddScore(int baseAmount, int sessionMultiplier = 1)
    {
        int amount = Mathf.RoundToInt(baseAmount * sessionMultiplier);

        CurrentScore += amount;
        RmbCooldownScore += amount;

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
            SaveGameData();
            return true;
        }
        return false;
    }

    public bool UseRmbCooldownScore(int amount)
    {
        if (RmbCooldownScore >= amount)
        {
            RmbCooldownScore -= amount;
            UpdateScoreUI();
            SaveGameData();
            return true;
        }
        return false;
    }

    public void ResetScore()
    {
        CurrentScore = startingScore;
        RmbCooldownScore = startingScore;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(CurrentScore);
        SaveGameData();
    }

    // --- Difficulty Methods ---
    public void SetDifficulty(DifficultyMode mode)
    {
        currentMode = mode;

        if (gameData == null)
            gameData = new GameData();

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

    public void UpdateHighScore(int score)
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

        SaveGameData();
    }

    public int GetBestMultiplierForCurrentMode()
    {
        if (gameData == null) return 0;

        return currentMode switch
        {
            DifficultyMode.Easy => gameData.bestMultiplierEasy,
            DifficultyMode.Hard => gameData.bestMultiplierHard,
            DifficultyMode.Extreme => gameData.bestMultiplierExtreme,
            _ => 0
        };
    }

    public void UpdateBestMultiplier(int sessionMultiplier)
    {
        if (gameData == null) return;

        switch (currentMode)
        {
            case DifficultyMode.Easy:
                if (sessionMultiplier > gameData.bestMultiplierEasy) gameData.bestMultiplierEasy = sessionMultiplier;
                break;
            case DifficultyMode.Hard:
                if (sessionMultiplier > gameData.bestMultiplierHard) gameData.bestMultiplierHard = sessionMultiplier;
                break;
            case DifficultyMode.Extreme:
                if (sessionMultiplier > gameData.bestMultiplierExtreme) gameData.bestMultiplierExtreme = sessionMultiplier;
                break;
        }
        SaveGameData();
    }

    public void ResetHighScoreAndBestMultiplier()
    {
        if (gameData == null) return;

        switch (currentMode)
        {
            case DifficultyMode.Easy:
                gameData.highScoreEasy = 0;
                gameData.bestMultiplierEasy = 0;
                break;
            case DifficultyMode.Hard:
                gameData.highScoreHard = 0;
                gameData.bestMultiplierHard = 0;
                break;
            case DifficultyMode.Extreme:
                gameData.highScoreExtreme = 0;
                gameData.bestMultiplierExtreme = 0;
                break;
        }
        SaveGameData();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            int bestMultiplier = GetBestMultiplierForCurrentMode();
            scoreText.text = $"Score: {CurrentScore} | Multiplier: x{bestMultiplier} | Difficulty: {currentMode} | RMB: {RmbCooldownScore}";
        }
    }

    // --- Robust Save/Load ---
    private void SaveGameData()
    {
        if (gameData == null)
            gameData = new GameData();

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
    }

    private void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData(); // ensure persistence on quit
    }
}
