using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text progressText;

    // Session-only multiplier
    private int multiplier = 1;

    // Requirements per difficulty
    private Dictionary<DifficultyMode, int> requirements = new Dictionary<DifficultyMode, int>()
    {
        {DifficultyMode.Easy, 5},
        {DifficultyMode.Hard, 10},
        {DifficultyMode.Extreme, 20}
    };

    // Track destroyed counts
    private Dictionary<string, int> destroyedCounts = new Dictionary<string, int>()
    {
        {"Boss", 0},
        {"MinionCapsule", 0},
        {"MinionCube", 0},
        {"MinionCylinder", 0},
        {"MinionSphere", 0}
    };

    // Points per object
    private Dictionary<string, int> points = new Dictionary<string, int>()
    {
        {"Boss", 1},
        {"MinionCapsule", 2},
        {"MinionCube", 3},
        {"MinionCylinder", 2},
        {"MinionSphere", 3}
    };

    private DifficultyMode currentDifficulty = DifficultyMode.Easy;
    private GameData gameData;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Load difficulty from GameData JSON
        gameData = GameDataManager.Load();
        if (gameData == null)
        {
            Debug.LogWarning("GameDataManager.Load() returned null, creating defaults.");
            gameData = new GameData();
        }

        // Always use ScoreManager’s difficulty if available
        if (ScoreManager.Instance != null)
        {
            currentDifficulty = ScoreManager.Instance.currentMode;
        }
        else
        {
            currentDifficulty = gameData.selectedDifficulty;
        }

        // Force ScoreManager to match GameManager difficulty
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetDifficulty(currentDifficulty);
        }

        Debug.Log("GameManager difficulty set to: " + currentDifficulty);

        // Reset score and multiplier at the start of each run
        multiplier = 1;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        ResetCounts();
        UpdateUI();
    }

    public void ObjectDestroyed(string objectType)
    {
        if (!destroyedCounts.ContainsKey(objectType)) return;

        // Add score through ScoreManager (with multiplier applied)
        int gained = points[objectType] * multiplier;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(gained, multiplier);

        destroyedCounts[objectType]++;

        if (PuzzleSolved())
        {
            multiplier++;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.UpdateBestMultiplier(multiplier);

            ResetCounts();
        }

        UpdateUI();
    }

    bool PuzzleSolved()
    {
        int requirement = requirements[currentDifficulty];
        foreach (var kvp in destroyedCounts)
        {
            if (kvp.Value < requirement) return false;
        }
        return true;
    }

    void ResetCounts()
    {
        var keys = new List<string>(destroyedCounts.Keys);
        foreach (var key in keys)
        {
            destroyedCounts[key] = 0;
        }
    }

    void UpdateUI()
    {
        if (scoreText != null && ScoreManager.Instance != null)
        {
            scoreText.text =
                $"Score: {ScoreManager.Instance.CurrentScore} (x{multiplier}) | Difficulty: {currentDifficulty}";
        }

        if (progressText != null)
        {
            int requirement = requirements[currentDifficulty];
            progressText.text =
                $"SCORE MULTIPLIER OBJECTIVE\n" +
                $"Boss: {destroyedCounts["Boss"]}/{requirement}\n" +
                $"Capsule: {destroyedCounts["MinionCapsule"]}/{requirement}\n" +
                $"Cube: {destroyedCounts["MinionCube"]}/{requirement}\n" +
                $"Cylinder: {destroyedCounts["MinionCylinder"]}/{requirement}\n" +
                $"Sphere: {destroyedCounts["MinionSphere"]}/{requirement}";
        }
    }

    // --- Called when the run ends (player dies, level complete, etc.)
    public void EndGame()
    {
        if (ScoreManager.Instance != null)
        {
            // Save best multiplier record
            ScoreManager.Instance.UpdateBestMultiplier(multiplier);

            // Save high score at end of run
            ScoreManager.Instance.UpdateHighScore(ScoreManager.Instance.CurrentScore);
        }

        Debug.Log("Game ended. Best multiplier and high score updated.");
    }

    // --- Expose session multiplier for UI (e.g., GameOverUI)
    public int GetSessionMultiplier()
    {
        return multiplier;
    }
}
