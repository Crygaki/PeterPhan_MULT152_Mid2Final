using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   // background music (lower volume)
    public AudioSource sfxSource;     // sound effects (higher volume)

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip gameOverMusic;

    [Header("UI Sound Clips")]
    public AudioClip mouseOverClip;
    public AudioClip clickClip;

    [Header("Game Over SFX")]
    public AudioClip gameOverSfx;

    [Header("Apple SFX")]
    public AudioClip normalAppleClip;
    public AudioClip goldAppleClip;
    public AudioClip poisonAppleClip;

    [Header("Basket SFX")]
    public AudioClip basketRemovedClip;

    [Header("Level Up SFX")]
    public AudioClip levelUpClip;

    [Header("Apple Drop SFX")]
    public AudioClip appleDropClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "Scene_0":
                PlayMusic(gameplayMusic);
                break;
            case "GameOverScene":
                PlayMusic(gameOverMusic);
                PlayGameOverSfx(); // plays on top of music
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // --- UI Sound Effects ---
    public void PlayMouseOver()
    {
        if (mouseOverClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(mouseOverClip);
        }
    }

    public void PlayClick()
    {
        if (clickClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }

    // --- Game Over Sound Effect ---
    public void PlayGameOverSfx()
    {
        if (gameOverSfx != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(gameOverSfx);
        }
    }

    // --- Apple Sound Effects ---
    public void PlayAppleSfx(AppleType type)
    {
        if (sfxSource == null) return;

        switch (type)
        {
            case AppleType.Normal:
                if (normalAppleClip != null) sfxSource.PlayOneShot(normalAppleClip);
                break;
            case AppleType.Gold:
                if (goldAppleClip != null) sfxSource.PlayOneShot(goldAppleClip);
                break;
            case AppleType.Poison:
                if (poisonAppleClip != null) sfxSource.PlayOneShot(poisonAppleClip);
                break;
        }
    }

    // --- Basket Removed Sound Effect ---
    public void PlayBasketRemovedSfx()
    {
        if (basketRemovedClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(basketRemovedClip);
        }
    }

    // --- Level Up Sound Effect ---
    public void PlayLevelUpSfx()
    {
        if (levelUpClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(levelUpClip);
        }
    }

    // --- Apple Drop Sound Effect ---
    public void PlayAppleDropSfx()
    {
        if (appleDropClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(appleDropClip);
        }
    }
}
