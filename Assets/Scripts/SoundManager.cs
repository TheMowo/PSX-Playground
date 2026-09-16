using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Mixer Settings")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Music Settings")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AudioSource musicSource; // assign in Inspector if desired

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip uiHoverClip;
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioSource sfxSource; // assign in Inspector if desired

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create sources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign mixer groups if provided
        if (musicMixerGroup != null)
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        if (sfxMixerGroup != null)
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Start()
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
            musicSource.volume = 1f;
        }
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
        musicSource.loop = true;

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
