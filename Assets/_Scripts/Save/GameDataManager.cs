using UnityEngine;
using System.IO;

public static class GameDataManager
{
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "gameData.json");
    private const int CurrentVersion = 1; // bump this when schema changes

    public static void Save(GameData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Tried to save null GameData. Creating defaults instead.");
            data = CreateDefaultData();
        }

        // Always update version before saving
        data.dataVersion = CurrentVersion;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public static GameData Load()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(json))
                {
                    GameData loaded = JsonUtility.FromJson<GameData>(json);

                    if (loaded != null)
                    {
                        // Check version compatibility
                        if (loaded.dataVersion != CurrentVersion)
                        {
                            Debug.LogWarning($"GameData version mismatch. Found {loaded.dataVersion}, expected {CurrentVersion}. Resetting to defaults.");
                            GameData defaultData = CreateDefaultData();
                            Save(defaultData);
                            return defaultData;
                        }

                        return loaded;
                    }
                }

                Debug.LogWarning("GameData file was empty or invalid. Resetting to defaults.");
            }
            else
            {
                Debug.Log("No GameData file found. Creating defaults.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading GameData: " + ex.Message);
        }

        // Always return a valid object
        GameData defaultDataFallback = CreateDefaultData();
        Save(defaultDataFallback);
        return defaultDataFallback;
    }

    private static GameData CreateDefaultData()
    {
        return new GameData
        {
            dataVersion = CurrentVersion,
            selectedDifficulty = DifficultyMode.Easy,
            highScoreEasy = 0,
            highScoreHard = 0,
            highScoreExtreme = 0,
            bestMultiplierEasy = 0,
            bestMultiplierHard = 0,
            bestMultiplierExtreme = 0
        };
    }
}
