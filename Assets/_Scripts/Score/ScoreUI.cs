using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text totalAccumulationText;

    private int lastLevel = 1; // track previous level

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ScoreManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = "Score: " + ScoreManager.Instance.score.ToString("#,0");

        if (highScoreText != null)
            highScoreText.text = "High Score (" + ScoreManager.Instance.currentMode + "): "
                + ScoreManager.Instance.GetHighScoreForCurrentMode().ToString("#,0");

        if (totalAccumulationText != null)
            totalAccumulationText.text = "Total Points: " + ScoreManager.Instance.sessionTotalScore.ToString("#,0");

        if (levelText != null)
        {
            levelText.text = "Level: " + ScoreManager.Instance.level.ToString();

            // Check if level increased
            if (ScoreManager.Instance.level > lastLevel)
            {
                TriggerLevelUpEffect();
                lastLevel = ScoreManager.Instance.level;
            }
        }
    }

    private void TriggerLevelUpEffect()
    {
        // Flash the level text in green
        levelText.color = Color.green;

        // Reset back to white after 0.5 seconds
        Invoke("ResetLevelTextColor", 0.5f);

        // Enlarge/shrink text for a "pop" effect
        levelText.fontSize += 10;
        Invoke("ResetLevelTextSize", 0.5f);
    }

    private void ResetLevelTextColor()
    {
        levelText.color = Color.white;
    }

    private void ResetLevelTextSize()
    {
        levelText.fontSize -= 10;
    }
}
