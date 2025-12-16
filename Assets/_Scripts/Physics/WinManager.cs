using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public static WinManager instance;

    // Required destruction counts
    private Dictionary<string, int> requiredCounts = new Dictionary<string, int>()
    {
        { "Boss", 5 },
        { "MinionCapsule", 1 },
        { "MinionCube", 1 },
        { "MinionCylinder", 1 },
        { "MinionSphere", 1 },
    };

    // Current destruction counts
    private Dictionary<string, int> destroyedCounts = new Dictionary<string, int>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        foreach (var key in requiredCounts.Keys)
            destroyedCounts[key] = 0;
    }

    public void RegisterDestruction(string objectType)
    {
        if (!destroyedCounts.ContainsKey(objectType)) return;

        destroyedCounts[objectType]++;
        Debug.Log($"Destroyed {objectType}: {destroyedCounts[objectType]} / {requiredCounts[objectType]}");

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        foreach (var key in requiredCounts.Keys)
        {
            if (destroyedCounts[key] < requiredCounts[key])
                return;
        }

        WinLevel();
    }

    private void WinLevel()
    {
        Debug.Log("Level Complete! All required objects destroyed.");
        SceneManager.LoadScene("WinScene");
    }
}
