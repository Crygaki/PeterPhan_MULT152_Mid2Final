using UnityEngine;
using System.IO;

public static class SettingsManager
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "settings.json");

    public static void SaveSettings(SettingsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public static SettingsData LoadSettings()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            // Default settings if no file exists
            return new SettingsData { selectedDifficulty = DifficultyMode.Easy };
        }
    }
}
