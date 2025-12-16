using UnityEditor;
using UnityEngine;
using System.IO;

public class CleanBuild
{
    [MenuItem("Build Tools/Clean and Build %#b")] // Ctrl/Cmd + Shift + B
    public static void CleanAndBuild()
    {
        // Close play mode if running
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;

        // Paths
        string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string libraryBeePath = Path.Combine(projectPath, "Library/Bee");
        string buildOutputPath = Path.Combine(projectPath, "Build");

        // Delete Bee cache
        if (Directory.Exists(libraryBeePath))
        {
            Directory.Delete(libraryBeePath, true);
            Debug.Log("Deleted Library/Bee cache.");
        }

        // Delete old build output
        if (Directory.Exists(buildOutputPath))
        {
            Directory.Delete(buildOutputPath, true);
            Debug.Log("Deleted old Build folder.");
        }

        // Delete global Bee cache
        string globalBeeCache = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".unity/bee");
        if (Directory.Exists(globalBeeCache))
        {
            Directory.Delete(globalBeeCache, true);
            Debug.Log("Deleted global Bee cache.");
        }

        // Scenes to include in build (in order)
        string[] scenes = {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Scene_0.unity",
            "Assets/Scenes/GameOverScene.unity"
        };

        // Perform fresh build
        BuildPipeline.BuildPlayer(
            scenes,
            buildOutputPath + "/PickingPuzzle.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );

        Debug.Log("Fresh build completed with clean cache!");
    }
}
