using TMPro;
using UnityEngine;

public class GlyphRow : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;

    private const string GlyphSet = "0123456789";

    private char[] characters;
    private bool[] cleared;

    private int characterCount;
    private int centerIndex;

    private float decodeRadius;

    private bool decoding;
    private bool fullyDecoded;

    public bool FullyDecoded => fullyDecoded;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    public void ResetRow(int width)
    {
        characterCount = width;

        characters = new char[characterCount];
        cleared = new bool[characterCount];

        centerIndex = characterCount / 2;

        decodeRadius = 0f;
        decoding = false;
        fullyDecoded = false;

        FillRandom();
        UpdateText();
    }

    public void BeginDecode()
    {
        decoding = true;
    }

    public void StopDecode()
    {
        decoding = false;
    }

    public void RandomizeGlyphs(int changesPerTick)
    {
        if (fullyDecoded)
        {
            return;
        }

        for (int i = 0; i < changesPerTick; i++)
        {
            int index = Random.Range(0, characterCount);

            if (cleared[index])
            {
                continue;
            }

            characters[index] = RandomGlyph();
        }

        UpdateText();
    }

    public void DecodeStep(float radiusGrowth, float waveFalloffWidth, float cleanupDistanceMultiplier,
        AnimationCurve clearChanceCurve)
    {
        if (!decoding || fullyDecoded)
        {
            return;
        }

        decodeRadius += radiusGrowth;

        bool hasVisibleGlyphs = false;

        float cleanupDistance = waveFalloffWidth * cleanupDistanceMultiplier;

        for (int i = 0; i < characterCount; i++)
        {
            if (cleared[i])
            {
                continue;
            }

            float distance = Mathf.Abs(i - centerIndex);

            float waveFrontDistance = decodeRadius - distance;

            if (waveFrontDistance >= cleanupDistance)
            {
                cleared[i] = true;
                characters[i] = ' ';
                continue;
            }

            if (waveFrontDistance >= 0f)
            {
                float normalized = Mathf.Clamp01(waveFrontDistance / waveFalloffWidth);

                float clearChance = clearChanceCurve.Evaluate(normalized);

                if (Random.value <= clearChance)
                {
                    cleared[i] = true;
                    characters[i] = ' ';
                    continue;
                }
            }

            hasVisibleGlyphs = true;
        }

        fullyDecoded = !hasVisibleGlyphs;

        UpdateText();
    }

    private void FillRandom()
    {
        for (int i = 0; i < characterCount; i++)
        {
            characters[i] = RandomGlyph();
            cleared[i] = false;
        }
    }

    private char RandomGlyph()
    {
        return GlyphSet[Random.Range(0, GlyphSet.Length)];
    }

    private void UpdateText()
    {
        if (textComponent == null)
        {
            return;
        }

        textComponent.text = new string(characters);
    }
}