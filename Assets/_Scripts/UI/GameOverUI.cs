using UnityEngine;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text finalResultText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text newRecordText;

    void Start()
    {
        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false); // hidden at start
        }
        ShowResults();
    }

    private void ShowResults()
    {
        if (ScoreManager.Instance == null) return;

        // Final score and session multiplier
        int finalScore = ScoreManager.Instance.CurrentScore;
        int finalMultiplier = GameManager.instance != null ? GameManager.instance.GetSessionMultiplier() : 1;

        // Best multiplier record
        int bestMultiplier = ScoreManager.Instance.GetBestMultiplierForCurrentMode();

        if (finalResultText != null)
        {
            finalResultText.text =
                "Final Score: " + finalScore.ToString("#,0") +
                "\nSession Multiplier: x" + finalMultiplier +
                "\nBest Multiplier: x" + bestMultiplier;
        }

        // High score record
        int hs = ScoreManager.Instance.GetHighScoreForCurrentMode();

        if (highScoreText != null)
        {
            highScoreText.text =
                "High Score (" + ScoreManager.Instance.currentMode + "): " +
                hs.ToString("#,0") +
                "\nBest Multiplier: x" + bestMultiplier;
        }

        // Check if new records were set
        bool isNewScore = finalScore >= hs;
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
}
