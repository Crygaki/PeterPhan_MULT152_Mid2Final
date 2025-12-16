using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public static UIControl Instance;   // Singleton reference

    void Awake()
    {
        // Ensure only one UIControl exists across all scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void StartGame()
    {
        // Load settings from JSON
        SettingsData settings = SettingsManager.LoadSettings();

        if (ScoreManager.Instance != null)
        {
            // Apply difficulty directly from enum
            ScoreManager.Instance.SetDifficulty(settings.selectedDifficulty);
        }

        // Load gameplay scene
        SceneManager.LoadScene("StickyBomb");
    }

    public void PlayAgain()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
