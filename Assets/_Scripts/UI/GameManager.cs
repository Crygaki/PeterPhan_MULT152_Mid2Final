using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text progressText;

    private int score = 0;
    private int multiplier = 1;

    // Requirements per difficulty (enum keys instead of strings)
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

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Load difficulty from JSON settings
        SettingsData settings = SettingsManager.LoadSettings();
        currentDifficulty = settings.selectedDifficulty;

        ResetCounts();
        UpdateUI();
    }

    public void ObjectDestroyed(string objectType)
    {
        if (!destroyedCounts.ContainsKey(objectType)) return;

        score += points[objectType] * multiplier;
        destroyedCounts[objectType]++;

        if (PuzzleSolved())
        {
            multiplier++;
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
        if (scoreText != null)
            scoreText.text = $"Score: {score} (x{multiplier})";

        if (progressText != null)
        {
            int requirement = requirements[currentDifficulty];
            progressText.text =
                $"Boss: {destroyedCounts["Boss"]}/{requirement}\n" +
                $"Capsule: {destroyedCounts["MinionCapsule"]}/{requirement}\n" +
                $"Cube: {destroyedCounts["MinionCube"]}/{requirement}\n" +
                $"Cylinder: {destroyedCounts["MinionCylinder"]}/{requirement}\n" +
                $"Sphere: {destroyedCounts["MinionSphere"]}/{requirement}";
        }
    }
}
