using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GhostCreditsSingleTMP : MonoBehaviour
{
    private const int Columns = 51;
    private const int Rows = 15;
    private const string GlyphSet = "0123456789";

    [Header("References")]
    [SerializeField] private TMP_Text textComponent;

    [Header("Random")]
    [SerializeField] private int randomSeed = 2501;
    [SerializeField] private int precomputedGlyphCount = 8192;

    [Header("Randomization")]
    [SerializeField] private float randomizeInterval = 0.03f;
    [SerializeField] private int glyphChangesPerTick = 24;

    [Header("Decode Wave")]
    [SerializeField] private float rowExpansionInterval = 0.15f;
    [SerializeField] private float decodeInterval = 0.03f;
    [SerializeField] private float decodeDuration = 0.8f;
    [SerializeField] private float waveFalloffWidth = 4f;
    [SerializeField] private float cleanupDistanceMultiplier = 2f;

    [SerializeField]
    private AnimationCurve clearChanceCurve = new(
        new Keyframe(0f, 0f),
        new Keyframe(0.2f, 0.1f),
        new Keyframe(0.4f, 0.35f),
        new Keyframe(0.7f, 0.8f),
        new Keyframe(1f, 0.95f)
    );

    [Header("Reveal")]
    [SerializeField] private GameObject revealObject;
    [SerializeField] private float revealDelay = 0.5f;

    [Header("Auto Start")]
    [SerializeField] private bool startDecodeOnAwake;

    private char[] displayBuffer;
    private bool[] cleared;

    private bool[] rowDecoding;
    private float[] rowRadii;
    private int[] rowStarts;

    private char[] precomputedGlyphs;
    private int glyphCursor;

    private bool decodingActive;

    private int randomizeCursor;
    private int totalGlyphSlots;

    private float radiusGrowthPerStep;

    private System.Random glyphRandom;

    private Coroutine randomizeRoutine;
    private Coroutine decodeRoutine;
    private Coroutine expandRoutine;
    private Coroutine revealRoutine;

    private void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }
    }

    private void Start()
    {
        InitializeRandom();
        CalculateDecodeSpeed();
        BuildGrid();

        randomizeRoutine = StartCoroutine(RandomizeRoutine());

        if (startDecodeOnAwake)
        {
            StartDecoding();
        }
    }

    private void InitializeRandom()
    {
        glyphRandom = new System.Random(randomSeed);

        precomputedGlyphs = new char[precomputedGlyphCount];

        for (int i = 0; i < precomputedGlyphs.Length; i++)
        {
            precomputedGlyphs[i] = GlyphSet[glyphRandom.Next(GlyphSet.Length)];
        }

        glyphCursor = 0;
    }

    private void BuildGrid()
    {
        int totalCharacters = (Columns * Rows) + (Rows - 1);

        displayBuffer = new char[totalCharacters];
        cleared = new bool[totalCharacters];

        rowStarts = new int[Rows];
        rowDecoding = new bool[Rows];
        rowRadii = new float[Rows];

        totalGlyphSlots = Columns * Rows;
        randomizeCursor = 0;

        int bufferIndex = 0;

        for (int row = 0; row < Rows; row++)
        {
            rowStarts[row] = bufferIndex;

            for (int col = 0; col < Columns; col++)
            {
                displayBuffer[bufferIndex] = NextGlyph();
                bufferIndex++;
            }

            if (row < Rows - 1)
            {
                displayBuffer[bufferIndex] = '\n';
                cleared[bufferIndex] = true;
                bufferIndex++;
            }
        }

        textComponent.SetCharArray(displayBuffer);
    }

    public void StartDecoding()
    {
        if (decodingActive)
        {
            return;
        }

        decodingActive = true;

        if (revealRoutine != null)
        {
            StopCoroutine(revealRoutine);
        }

        revealRoutine = StartCoroutine(RevealRoutine());
        decodeRoutine = StartCoroutine(DecodeRoutine());
        expandRoutine = StartCoroutine(ExpandDecodeWave());
    }

    public void StopDecoding()
    {
        decodingActive = false;

        if (decodeRoutine != null)
        {
            StopCoroutine(decodeRoutine);
            decodeRoutine = null;
        }

        if (expandRoutine != null)
        {
            StopCoroutine(expandRoutine);
            expandRoutine = null;
        }

        if (revealRoutine != null)
        {
            StopCoroutine(revealRoutine);
            revealRoutine = null;
        }
    }

    private void CalculateDecodeSpeed()
    {
        float maxRadius = Columns / 2f;

        int decodeSteps = Mathf.Max(1, Mathf.CeilToInt(decodeDuration / decodeInterval));

        radiusGrowthPerStep = maxRadius / decodeSteps;
    }

    public void ResetCredits()
    {
        StopDecoding();

        if (revealObject != null)
        {
            revealObject.SetActive(false);
        }

        InitializeRandom();
        CalculateDecodeSpeed();
        BuildGrid();
    }

    public void RestartCredits()
    {
        ResetCredits();
        StartDecoding();
    }

    private IEnumerator RandomizeRoutine()
    {
        WaitForSeconds wait = new(randomizeInterval);

        while (true)
        {
            bool dirty = false;

            for (int i = 0; i < glyphChangesPerTick; i++)
            {
                randomizeCursor++;

                if (randomizeCursor >= totalGlyphSlots)
                {
                    randomizeCursor = 0;
                }

                int row = randomizeCursor / Columns;
                int column = randomizeCursor % Columns;
                int index = rowStarts[row] + column;

                if (cleared[index])
                {
                    continue;
                }

                char glyph = NextGlyph();

                if (displayBuffer[index] == glyph)
                {
                    continue;
                }

                displayBuffer[index] = glyph;
                dirty = true;
            }

            if (dirty)
            {
                textComponent.SetCharArray(displayBuffer);
            }

            yield return wait;
        }
    }

    private IEnumerator DecodeRoutine()
    {
        WaitForSeconds wait = new(decodeInterval);

        while (true)
        {
            bool dirty = false;

            for (int row = 0; row < Rows; row++)
            {
                if (!rowDecoding[row])
                {
                    continue;
                }

                dirty |= DecodeRow(row);
            }

            if (dirty)
            {
                textComponent.SetCharArray(displayBuffer);
            }

            yield return wait;
        }
    }

    private bool DecodeRow(int row)
    {
        bool changed = false;

        rowRadii[row] += radiusGrowthPerStep;

        float cleanupDistance = waveFalloffWidth * cleanupDistanceMultiplier;
        int centerColumn = Columns / 2;
        int rowStart = rowStarts[row];

        for (int col = 0; col < Columns; col++)
        {
            int index = rowStart + col;

            if (cleared[index])
            {
                continue;
            }

            float distance = Mathf.Abs(col - centerColumn);
            float waveFrontDistance = rowRadii[row] - distance;

            if (waveFrontDistance >= cleanupDistance)
            {
                cleared[index] = true;
                displayBuffer[index] = ' ';
                changed = true;
                continue;
            }

            if (waveFrontDistance < 0f)
            {
                continue;
            }

            float normalized = Mathf.Clamp01(waveFrontDistance / waveFalloffWidth);
            float chance = clearChanceCurve.Evaluate(normalized);

            if (UnityEngine.Random.value <= chance)
            {
                cleared[index] = true;
                displayBuffer[index] = ' ';
                changed = true;
            }
        }

        return changed;
    }

    private IEnumerator ExpandDecodeWave()
    {
        WaitForSeconds wait = new(rowExpansionInterval);

        int centerRow = Rows / 2;

        rowDecoding[centerRow] = true;

        for (int distance = 1; distance <= centerRow; distance++)
        {
            yield return wait;

            int upper = centerRow - distance;
            int lower = centerRow + distance;

            if (upper >= 0)
            {
                rowDecoding[upper] = true;
            }

            if (lower < Rows)
            {
                rowDecoding[lower] = true;
            }
        }

        expandRoutine = null;
    }

    private IEnumerator RevealRoutine()
    {
        if (revealObject == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(revealDelay);

        revealObject.SetActive(true);

        revealRoutine = null;
    }

    private char NextGlyph()
    {
        char glyph = precomputedGlyphs[glyphCursor];

        glyphCursor++;

        if (glyphCursor >= precomputedGlyphs.Length)
        {
            glyphCursor = 0;
        }

        return glyph;
    }

    [Button(ButtonSizes.Large)]
    public void StartButton()
    {
        StartDecoding();
    }

    [Button(ButtonSizes.Large)]
    public void StopButton()
    {
        StopDecoding();
    }

    [Button(ButtonSizes.Large)]
    public void ResetButton()
    {
        ResetCredits();
    }

    [Button(ButtonSizes.Large)]
    public void RestartButton()
    {
        RestartCredits();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        decodeInterval = Mathf.Max(0.01f, decodeInterval);
        decodeDuration = Mathf.Max(decodeInterval, decodeDuration);
    }

#endif
}