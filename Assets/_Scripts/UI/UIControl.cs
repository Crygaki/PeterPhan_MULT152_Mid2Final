using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIControl : MonoBehaviour
{
    public static UIControl Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // full screen black overlay
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private TMP_Text loadingText;        // "Loading..." text
    [SerializeField] private Image loadingSpinner;        // spinner image

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (loadingText != null)
            loadingText.gameObject.SetActive(false);

        if (loadingSpinner != null)
            loadingSpinner.gameObject.SetActive(false);
    }

    // --- Scene Management with Fade ---
    public void StartGame()
    {
        GameData data = GameDataManager.Load();
        if (ScoreManager.Instance != null && data != null)
            ScoreManager.Instance.SetDifficulty(data.selectedDifficulty);

        StartCoroutine(FadeAndLoadScene("StickyBomb"));
    }

    public void PlayAgain()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        StartCoroutine(FadeAndLoadScene("MainMenu"));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Fade Coroutine with Loading UI ---
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadeCanvasGroup == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        // Enable loading UI
        if (loadingText != null) loadingText.gameObject.SetActive(true);
        if (loadingSpinner != null) loadingSpinner.gameObject.SetActive(true);

        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Fade back in
        yield return StartCoroutine(Fade(0f));

        // Hide loading UI
        if (loadingText != null) loadingText.gameObject.SetActive(false);
        if (loadingSpinner != null) loadingSpinner.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (fadeCanvasGroup == null) yield break; // stop if destroyed

            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);

            // Rotate spinner while fading
            if (loadingSpinner != null && loadingSpinner.gameObject.activeSelf)
                loadingSpinner.transform.Rotate(Vector3.forward, -360f * Time.deltaTime);

            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = targetAlpha;
    }
}
