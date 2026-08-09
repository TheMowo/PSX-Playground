using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SimpleFilter : MonoBehaviour
{
    [SerializeField] private Material material;
    //[SerializeField] private RawImage rawImage;

    //public void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    rawImage.texture = source;
    //    material.SetTexture("_MainTex", source);
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleEffect();
        }
    }

    [PropertySpace(25), Button]
    [ContextMenu("Toggle Effect")]
    public void ToggleEffect()
    {
        if (material.IsKeywordEnabled("TOGGLE_EFFECT"))
        {
            material.DisableKeyword("TOGGLE_EFFECT");
        }
        else
        {
            material.EnableKeyword("TOGGLE_EFFECT");
        }
        Debug.Log($"Effect toggled - Status: {material.IsKeywordEnabled("TOGGLE_EFFECT")}");
    }
}
