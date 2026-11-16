using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostCreditsController : MonoBehaviour
{
    [Header("Rows")]
    [SerializeField] private List<GlyphRow> rows = new();

    [Header("Grid Settings")]
    [SerializeField] private int rowWidth = 51;

    [Header("Glyph Animation")]
    [SerializeField] private float randomizeInterval = 0.03f;
    [SerializeField] private int glyphChangesPerTick = 6;

    [Header("Decode Wave")]
    [SerializeField] private float rowExpansionInterval = 0.15f;
    [SerializeField] private float decodeInterval = 0.03f;
    [SerializeField] private float radiusGrowthPerStep = 1.25f;

    [SerializeField]
    private float waveFalloffWidth = 4f;

    [SerializeField]
    private float cleanupDistanceMultiplier = 2f;

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

    private bool decodingActive;

    private Coroutine randomizeRoutine;
    private Coroutine decodeRoutine;
    private Coroutine expandDecodeWaveRoutine;
    private Coroutine revealRoutine;

    private void Awake()
    {
        rows = GetComponentsInChildren<GlyphRow>(true).ToList();
    }

    private void Start()
    {
        ResetCredits();

        randomizeRoutine = StartCoroutine(RandomizeRoutine());

        if (startDecodeOnAwake)
        {
            StartDecoding();
        }
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
        expandDecodeWaveRoutine = StartCoroutine(ExpandDecodeWave());
    }

    public void StopDecoding()
    {
        decodingActive = false;

        if (decodeRoutine != null)
        {
            StopCoroutine(decodeRoutine);
            decodeRoutine = null;
        }

        if (expandDecodeWaveRoutine != null)
        {
            StopCoroutine(expandDecodeWaveRoutine);
            expandDecodeWaveRoutine = null;
        }

        if (revealRoutine != null)
        {
            StopCoroutine(revealRoutine);
            revealRoutine = null;
        }

        foreach (GlyphRow row in rows)
        {
            row.StopDecode();
        }
    }

    public void ResetCredits()
    {
        StopDecoding();

        if (revealObject != null)
        {
            revealObject.SetActive(false);
        }

        foreach (GlyphRow row in rows)
        {
            row.ResetRow(rowWidth);
        }
    }

    public void RestartCredits()
    {
        ResetCredits();
        StartDecoding();
    }

    private IEnumerator RandomizeRoutine()
    {
        WaitForSeconds wait =
            new WaitForSeconds(randomizeInterval);

        while (true)
        {
            foreach (GlyphRow row in rows)
            {
                row.RandomizeGlyphs(glyphChangesPerTick);
            }

            yield return wait;
        }
    }

    private IEnumerator DecodeRoutine()
    {
        WaitForSeconds wait =
            new WaitForSeconds(decodeInterval);

        while (true)
        {
            foreach (GlyphRow row in rows)
            {
                row.DecodeStep(radiusGrowthPerStep, waveFalloffWidth, cleanupDistanceMultiplier, clearChanceCurve);
            }

            yield return wait;
        }
    }

    private IEnumerator ExpandDecodeWave()
    {
        WaitForSeconds wait = new WaitForSeconds(rowExpansionInterval);

        int centerRow = rows.Count / 2;

        rows[centerRow].BeginDecode();

        for (int distance = 1; distance <= centerRow; distance++)
        {
            yield return wait;

            int upper = centerRow - distance;
            int lower = centerRow + distance;

            if (upper >= 0)
            {
                rows[upper].BeginDecode();
            }

            if (lower < rows.Count)
            {
                rows[lower].BeginDecode();
            }
        }

        expandDecodeWaveRoutine = null;
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

    [PropertySpace(20)]
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
}