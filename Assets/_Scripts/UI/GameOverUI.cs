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

        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        int finalLevel = PlayerPrefs.GetInt("FinalLevel", 1);

        if (finalResultText != null)
            finalResultText.text = "Final Score: " + finalScore.ToString("#,0") +
                                   "\nLevel Reached: " + finalLevel;

        int hs = ScoreManager.Instance.GetHighScoreForCurrentMode();
        int hl = ScoreManager.Instance.GetHighScoreLevelForCurrentMode();

        if (highScoreText != null)
            highScoreText.text = "High Score (" + ScoreManager.Instance.currentMode + "): " +
                                 hs.ToString("#,0") +
                                 "\nLevel Reached: " + hl;

        bool isNewScore = PlayerPrefs.GetInt("IsNewScore", 0) == 1;
        bool isNewLevel = PlayerPrefs.GetInt("IsNewLevel", 0) == 1;

        if (isNewScore)
        {
            ShowNewRecord("New High Score!");
        }
        else if (isNewLevel)
        {
            ShowNewRecord("New Highest Level!");
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
