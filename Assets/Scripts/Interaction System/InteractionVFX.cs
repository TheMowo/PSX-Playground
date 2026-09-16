using UnityEngine;

public class InteractionVFX : MonoBehaviour
{
    [Header("Glow Settings")]
    public float hoverGlowIntensity = 2.5f;   // Glow when hovered
    public float normalGlowIntensity = 0f;    // Glow when not hovered
    public string glowProperty = "_GlowIntensity"; // Shader property name

    private Renderer rend;
    private Material mat;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Use material instance so we don’t affect shared material
            mat = rend.material;
            mat.SetFloat(glowProperty, normalGlowIntensity);
        }
    }

    void OnMouseEnter()
    {
        if (mat != null)
        {
            mat.SetFloat(glowProperty, hoverGlowIntensity);
        }
    }

    void OnMouseExit()
    {
        if (mat != null)
        {
            mat.SetFloat(glowProperty, normalGlowIntensity);
        }
    }
}
