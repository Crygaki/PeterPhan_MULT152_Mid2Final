using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Needed for PlayAgain

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text finalResultText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text newRecordText;

    void Start()
    {
        // Ensure cursor is visible and unlocked in GameOverScene
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false); // hidden at start
        }
        ShowResults();
    }

    void Update()
    {
        // Continuously enforce cursor state so it doesn't get hidden again
        if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void ShowResults()
    {
        if (ScoreManager.Instance == null) return;

        DifficultyMode mode = ScoreManager.Instance.currentMode;

        // --- Last run results ---
        int finalScore = ScoreManager.Instance.CurrentScore;
        int finalMultiplier = GameManager.instance != null ? GameManager.instance.GetSessionMultiplier() : 1;

        if (finalResultText != null)
        {
            finalResultText.text =
                $"LAST RUN ({mode})\n" +
                $"Score: {finalScore:#,0}\n" +
                $"Multiplier: x{finalMultiplier}";
        }

        // --- Highest records ---
        int highScore = ScoreManager.Instance.GetHighScoreForCurrentMode();
        int bestMultiplier = ScoreManager.Instance.GetBestMultiplierForCurrentMode();

        if (highScoreText != null)
        {
            highScoreText.text =
                $"HIGHEST RECORDS ({mode})\n" +
                $"High Score: {highScore:#,0}\n" +
                $"Best Multiplier: x{bestMultiplier}";
        }

        // --- Check if new records were set ---
        bool isNewScore = finalScore >= highScore;
        bool isNewMultiplier = finalMultiplier >= bestMultiplier;

        if (isNewScore && isNewMultiplier)
        {
            ShowNewRecord("New High Score & Multiplier!");
        }
        else if (isNewScore)
        {
            ShowNewRecord("New High Score!");
        }
        else if (isNewMultiplier)
        {
            ShowNewRecord("New Best Multiplier!");
        }
    }

    private void ShowNewRecord(string message)
    {
        if (newRecordText == null) return;

        newRecordText.gameObject.SetActive(true);
        newRecordText.text = message;
        newRecordText.color = Color.yellow;

        // Reset alpha to 0 instantly
        newRecordText.CrossFadeAlpha(0f, 0f, true);

        // Fade in over 0.5s
        newRecordText.CrossFadeAlpha(1f, 0.5f, false);

        // Bounce effect
        StartCoroutine(PopEffect());
    }

    private IEnumerator PopEffect()
    {
        Vector3 originalScale = newRecordText.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        float duration = 0.2f;
        float time = 0f;

        while (time < duration)
        {
            newRecordText.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < duration)
        {
            newRecordText.transform.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        newRecordText.transform.localScale = originalScale;
    }

    // --- New Methods for Buttons ---
    public void PlayAgain()
    {
        // Reload the StickyBomb gameplay scene
        SceneManager.LoadScene("StickyBomb");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Stop play mode in the Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application in a standalone build
        Application.Quit();
#endif
    }
}
