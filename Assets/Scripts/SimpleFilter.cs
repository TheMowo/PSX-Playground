//using MoreMountains.Feedbacks;
//using UnityEngine;

//[ExecuteInEditMode]
//public class SimpleFilter : MonoBehaviour
//{
//    [SerializeField] private Shader _shader;
//    //struct Shaders { private bool _toggle; private Shader _shader; } //WRONG

//    private Material _mat;

//    private void Awake()
//    {
//        _mat = new Material(_shader);
//    }

//    public void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//        _mat.SetTexture("_MainTex", source);
//        Graphics.Blit(source, destination, _mat);
//    }
//}

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SimpleFilter : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private RawImage rawImage;

    // Use this for initialization
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        rawImage.texture = source;
        material.SetTexture("_MainTex", source);
    }
}
