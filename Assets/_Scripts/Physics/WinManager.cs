using UnityEngine;
using System.Collections.Generic;

public class WinManager : MonoBehaviour
{
    public static WinManager instance;

    // Required destruction counts
    private Dictionary<string, int> requiredCounts = new Dictionary<string, int>()
    {
        { "Boss", 5 },
        { "Minion", 15 },
        //{ "GrayBall", 1 },
        //{ "BlackBall", 1 },
        //{ "WoodBarrel", 1 },
        //{ "YellowBox", 1 },
        //{ "RedBox", 1 }
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
        Object.FindFirstObjectByType<WinScreenManager>()?.ShowWinScreen();
    }
}