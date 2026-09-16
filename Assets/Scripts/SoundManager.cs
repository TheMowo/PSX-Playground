using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip uiHoverClip;
    [SerializeField] private AudioClip uiClickClip;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // --- Scene Events ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            StartCoroutine(FadeInMusic());
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        StartCoroutine(FadeOutMusic());
    }

    // --- Fade Coroutines ---
    private IEnumerator FadeInMusic()
    {
        musicSource.volume = 0f;
        musicSource.Play();

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = 1f;
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    // --- UI Sound Effects ---
    public void PlayUIHover()
    {
        if (uiHoverClip != null)
            sfxSource.PlayOneShot(uiHoverClip);
    }

    public void PlayUIClick()
    {
        if (uiClickClip != null)
            sfxSource.PlayOneShot(uiClickClip);
    }
}
