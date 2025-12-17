using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class UIControl : MonoBehaviour
{
    public static UIControl Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // full screen black overlay
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private TMP_Text loadingText;        // "Loading..." text
    [SerializeField] private Image loadingSpinner;        // spinner image

    [Header("Information Panel")]
    [SerializeField] private GameObject informationCanvas; // assign via Inspector

    [Header("Main Canvas")]
    [SerializeField] private GameObject mainCanvas; // assign via Inspector

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        // Ensure only one EventSystem exists (new API)
        var eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
                Destroy(eventSystems[i].gameObject);
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false; // prevent blocking clicks
        }

        if (loadingText != null) loadingText.gameObject.SetActive(false);
        if (loadingSpinner != null) loadingSpinner.gameObject.SetActive(false);
        if (informationCanvas != null) informationCanvas.SetActive(false);
    }

    // --- Scene Management with Fade ---
    public void StartGame()
    {
        // Hide the main canvas at the start of the game
        if (mainCanvas != null)
            mainCanvas.SetActive(false);

        // Apply difficulty from saved data
        GameData data = GameDataManager.Load();
        if (ScoreManager.Instance != null && data != null)
            ScoreManager.Instance.SetDifficulty(data.selectedDifficulty);

        StartCoroutine(FadeAndLoadScene("StickyBomb"));
    }

    // Restart the game from the beginning (load StickyBomb)
    public void PlayAgain()
    {
        // Reset score for a fresh run
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        // Apply difficulty from saved data
        GameData data = GameDataManager.Load();
        if (ScoreManager.Instance != null && data != null)
            ScoreManager.Instance.SetDifficulty(data.selectedDifficulty);

        // Hide main canvas if we’re coming from MainMenu
        if (mainCanvas != null)
            mainCanvas.SetActive(false);

        StartCoroutine(FadeAndLoadScene("StickyBomb"));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Toggle Information Canvas ---
    public void ToggleInformation()
    {
        if (informationCanvas != null)
        {
            bool isActive = informationCanvas.activeSelf;
            informationCanvas.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("Information Canvas not assigned in Inspector!");
        }
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

        // Safety: ensure overlay doesn’t block clicks after fade-in
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (fadeCanvasGroup == null) yield break;

            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);

            // Rotate spinner while fading
            if (loadingSpinner != null && loadingSpinner.gameObject.activeSelf)
                loadingSpinner.transform.Rotate(Vector3.forward, -360f * Time.deltaTime);

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;

        // Only block raycasts when fully faded to black
        fadeCanvasGroup.blocksRaycasts = (targetAlpha >= 1f);
    }
}
