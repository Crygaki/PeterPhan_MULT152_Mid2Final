using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scene_0");
    }

    public void PlayAgain()
    {
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("MainMenu");
    }

    // Called by the Quit button
    public void QuitGame()
    {
#if UNITY_EDITOR
        // If running inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If running a built standalone game
            Application.Quit();
#endif
    }
}
