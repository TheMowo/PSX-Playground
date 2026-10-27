using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private float minSize = 32f;
    [SerializeField] private float maxSize = 40f;
    [SerializeField] private string[] words;

    [Header("Movement")]
    [SerializeField] private float gravity = 8f;
    [SerializeField] private float horizontalSpread = 1f;
    [SerializeField] private float minUpwardVelocity = 3f;
    [SerializeField] private float maxUpwardVelocity = 5f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 1f;

    [Header("Scale Punch")]
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float punchDuration = 0.15f;

    private Vector3 velocity;
    private float timer;
    private Color textColor;

    private void Awake()
    {
        if (!text) text = GetComponentInChildren<TMP_Text>();
    }

    public void Initialize(float damage)
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.2f, 0.2f), 0f);

        velocity = new Vector3(Random.Range(-horizontalSpread, horizontalSpread), Random.Range(minUpwardVelocity, maxUpwardVelocity), 0f);

        float ratio = Mathf.InverseLerp(1f, 100f, damage);
        float variance = Random.Range(-2f, 2f);

        text.text = words[Random.Range(0, words.Length - 1)];
        text.fontSize = Mathf.Lerp(minSize, maxSize, ratio) + variance;

        textColor = text.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Float + Arc + Drop
        velocity += Vector3.down * gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Scale Punch
        float punchT = Mathf.Clamp01(timer / punchDuration);
        transform.localScale = Vector3.one * scaleCurve.Evaluate(punchT);

        // Fade Out
        float lifeT = timer / lifetime;
        float fadeT = Mathf.InverseLerp(0.6f, 1f, lifeT);

        textColor.a = 1f - fadeT;
        text.color = textColor;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
