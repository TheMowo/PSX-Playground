using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public sealed class GhostRevealText : MonoBehaviour
{
    public enum RevealOrder
    {
        LeftToRight,
        RightToLeft,
        Random
    }

    [Header("Text")]
    [SerializeField, TextArea(3, 10)] public string targetText = "MAJOR KUSANAGI";
    [SerializeField] private Color fadeWhite = Color.white;

    [SerializeField, TextArea(2, 5)]
    private string scrambleCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "!@#$%^&*+-=<>";

    [Header("Timing")]
    [SerializeField] private float startDelay = 0.25f;
    [SerializeField] private float revealDuration = 1.75f;
    [SerializeField] private float scrambleInterval = 0.04f;

    [Header("Completion Fade")]
    [SerializeField] private bool runFadeSequence = false;
    [SerializeField] private float postRevealDelay = 0.25f;
    [SerializeField] private float fadeToWhiteDuration = 1f;
    [SerializeField] private float holdBeforeFadeOut = 4f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Reveal")]
    [SerializeField] private RevealOrder revealOrder = RevealOrder.Random;
    [SerializeField, Range(0f, 1f)] private float initialCorrectFraction;
    [SerializeField] private bool scrambleSpaces;
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private int randomSeed;

    [Header("Events")]
    [SerializeField] private UnityEvent onRevealStarted;
    [SerializeField] private UnityEvent onRevealCompleted;
    [SerializeField] private UnityEvent onRevealHidden;

    private TMP_Text textComponent;
    private Coroutine revealCoroutine;

    private char[] targetCharacters;
    private char[] displayedCharacters;
    private bool[] revealedCharacters;

    private readonly List<int> revealableIndices = new();

    private System.Random random;
    private Color defaultColor;

#if UNITY_EDITOR

    private string previousTargetText;
    private string previousTmpText;

#endif

    public bool IsPlaying => revealCoroutine != null;

    public string TargetText => targetText;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        defaultColor = textComponent.color;
    }

    private void OnEnable()
    {
        if (playOnEnable)
            Play();
    }

    private void OnDisable()
    {
        Stop();
    }

    [ContextMenu("Play Reveal")]
    public void Play()
    {
        Play(targetText);
    }

    public void Play(string newTargetText)
    {
        targetText = newTargetText ?? string.Empty;

        Stop();

        textComponent.color = defaultColor;
        gameObject.SetActive(true);

        if (!isActiveAndEnabled)
            return;

        revealCoroutine = StartCoroutine(RevealRoutine());
    }

    public void SetTargetText(string newTargetText, bool playImmediately = true)
    {
        targetText = newTargetText ?? string.Empty;

        if (playImmediately)
            Play();
        else
            ShowCompletedText();
    }

    public void Stop()
    {
        if (revealCoroutine == null)
            return;

        StopCoroutine(revealCoroutine);
        revealCoroutine = null;

        textComponent.color = defaultColor;
    }

    public void SkipToEnd()
    {
        Stop();

        ShowCompletedText();

        onRevealCompleted?.Invoke();
    }

    public void Clear()
    {
        Stop();

        textComponent.color = defaultColor;
        textComponent.text = string.Empty;
    }

    private IEnumerator RevealRoutine()
    {
        InitializeReveal();

        textComponent.text = new string(displayedCharacters);

        if (startDelay > 0f)
            yield return WaitForSeconds(startDelay);

        onRevealStarted?.Invoke();

        float elapsed = 0f;
        float nextScrambleTime = 0f;
        int revealableCount = revealableIndices.Count;

        while (elapsed < revealDuration)
        {
            float normalizedTime = Mathf.Clamp01(elapsed / revealDuration);
            int targetRevealCount = Mathf.CeilToInt(normalizedTime * revealableCount);

            RevealUpTo(targetRevealCount);

            if (elapsed >= nextScrambleTime)
            {
                ScrambleUnrevealedCharacters();

                textComponent.text = new string(displayedCharacters);

                nextScrambleTime += scrambleInterval;
            }

            yield return null;

            elapsed += GetDeltaTime();
        }

        RevealUpTo(revealableCount);

        ShowCompletedText();

        if (!runFadeSequence)
            yield break;

        onRevealCompleted?.Invoke();

        yield return WaitForSeconds(postRevealDelay);

        Color startColor = defaultColor;

        float fadeElapsed = 0f;

        while (fadeElapsed < fadeToWhiteDuration)
        {
            fadeElapsed += GetDeltaTime();

            float t = Mathf.Clamp01(fadeElapsed / fadeToWhiteDuration);

            textComponent.color = Color.Lerp(startColor, fadeWhite, t);

            yield return null;
        }

        textComponent.color = fadeWhite;

        yield return WaitForSeconds(holdBeforeFadeOut);

        Color fadeOutStartColor = textComponent.color;

        fadeElapsed = 0f;

        while (fadeElapsed < fadeOutDuration)
        {
            fadeElapsed += GetDeltaTime();

            float t = Mathf.Clamp01(fadeElapsed / fadeOutDuration);

            textComponent.color = Color.Lerp(
                fadeOutStartColor,
                new Color(
                    fadeOutStartColor.r,
                    fadeOutStartColor.g,
                    fadeOutStartColor.b,
                    0f),
                t);

            yield return null;
        }

        Color finalColor = textComponent.color;
        finalColor.a = 0f;
        textComponent.color = finalColor;

        onRevealHidden?.Invoke();

        gameObject.SetActive(false);

        revealCoroutine = null;
    }

    private void InitializeReveal()
    {
        targetCharacters = targetText.ToCharArray();
        displayedCharacters = new char[targetCharacters.Length];
        revealedCharacters = new bool[targetCharacters.Length];

        revealableIndices.Clear();

        random = randomSeed == 0
            ? new System.Random()
            : new System.Random(randomSeed);

        for (int i = 0; i < targetCharacters.Length; i++)
        {
            char targetCharacter = targetCharacters[i];

            if (!IsRevealable(targetCharacter))
            {
                displayedCharacters[i] = targetCharacter;
                revealedCharacters[i] = true;
                continue;
            }

            revealableIndices.Add(i);
            displayedCharacters[i] = GetRandomScrambleCharacter();
        }

        SortRevealIndices();

        int initialRevealCount = Mathf.FloorToInt(revealableIndices.Count * initialCorrectFraction);

        RevealUpTo(initialRevealCount);
    }

    private void SortRevealIndices()
    {
        switch (revealOrder)
        {
            case RevealOrder.LeftToRight:
                revealableIndices.Sort();
                break;

            case RevealOrder.RightToLeft:
                revealableIndices.Sort((a, b) => b.CompareTo(a));
                break;

            case RevealOrder.Random:
                Shuffle(revealableIndices);
                break;
        }
    }

    private void RevealUpTo(int targetRevealCount)
    {
        targetRevealCount = Mathf.Clamp(targetRevealCount, 0, revealableIndices.Count);

        for (int i = 0; i < targetRevealCount; i++)
        {
            int characterIndex = revealableIndices[i];

            if (revealedCharacters[characterIndex])
                continue;

            revealedCharacters[characterIndex] = true;
            displayedCharacters[characterIndex] = targetCharacters[characterIndex];
        }
    }

    private void ScrambleUnrevealedCharacters()
    {
        for (int i = 0; i < displayedCharacters.Length; i++)
        {
            if (revealedCharacters[i])
                continue;

            displayedCharacters[i] = GetRandomScrambleCharacter();
        }
    }

    private char GetRandomScrambleCharacter()
    {
        if (string.IsNullOrEmpty(scrambleCharacters))
            return '0';

        return scrambleCharacters[random.Next(scrambleCharacters.Length)];
    }

    private bool IsRevealable(char character)
    {
        if (character == '\n' || character == '\r' || character == '\t')
            return false;

        if (character == ' ' && !scrambleSpaces)
            return false;

        return true;
    }

    private void ShowCompletedText()
    {
        textComponent.text = targetText;
    }

    private float GetDeltaTime()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }

    private IEnumerator WaitForSeconds(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return null;
            elapsed += GetDeltaTime();
        }
    }

    private void Shuffle(List<int> indices)
    {
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (indices[i], indices[swapIndex]) = (indices[swapIndex], indices[i]);
        }
    }

    private void Reset()
    {
        textComponent = GetComponent<TMP_Text>();

#if UNITY_EDITOR
        if (textComponent != null)
        {
            targetText = textComponent.text;
        }
#endif
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        revealDuration = Mathf.Max(0.01f, revealDuration);
        scrambleInterval = Mathf.Max(0.01f, scrambleInterval);
        startDelay = Mathf.Max(0f, startDelay);
        postRevealDelay = Mathf.Max(0f, postRevealDelay);
        fadeToWhiteDuration = Mathf.Max(0.01f, fadeToWhiteDuration);
        holdBeforeFadeOut = Mathf.Max(0f, holdBeforeFadeOut);
        fadeOutDuration = Mathf.Max(0.01f, fadeOutDuration);

        if (string.IsNullOrEmpty(scrambleCharacters))
            scrambleCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        if (textComponent == null)
            return;

        if (string.IsNullOrEmpty(previousTargetText))
            previousTargetText = targetText;

        if (string.IsNullOrEmpty(previousTmpText))
            previousTmpText = textComponent.text;

        if (targetText != previousTargetText)
        {
            textComponent.text = targetText;

            previousTargetText = targetText;
            previousTmpText = textComponent.text;

            UnityEditor.EditorUtility.SetDirty(textComponent);

            return;
        }

        if (textComponent.text != previousTmpText)
        {
            targetText = textComponent.text;

            previousTargetText = targetText;
            previousTmpText = textComponent.text;

            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}