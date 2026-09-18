using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ConvertToButton : MonoBehaviour
{
    [SerializeField] RenderTexture rentex;

    [Header("Button Events")]
    [SerializeField] private UnityEvent onClick;

    [Header("Button Colors (Hex)")]
    [SerializeField] private GameObject buttonVisualObject;
    [SerializeField] private string normalHex = "#525252";
    [SerializeField] private string hoverHex = "#808080";
    [SerializeField] private string pressedHex = "#00FF00";

    private Color normalColor;
    private Color hoverColor;
    private Color pressedColor;

    private Renderer _renderer;
    private bool _isHovering;

    private void Awake()
    {
        _renderer = buttonVisualObject.GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(normalHex, out normalColor);
        ColorUtility.TryParseHtmlString(hoverHex, out hoverColor);
        ColorUtility.TryParseHtmlString(pressedHex, out pressedColor);

        if (_renderer != null)
            _renderer.material.color = normalColor;
    }

    private void Update()
    {
        HandleHover();
        HandleClick();
    }

    private void HandleHover()
    {
        if (M_CameraManager.Instance == null) return;

        // Pass Input.mousePosition and the render texture
        Ray ray = M_CameraManager.Instance.GetActiveCameraRay(Input.mousePosition, rentex);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Ray hit: {hit.collider.gameObject.name} at {hit.point}");
            if (hit.collider.gameObject == gameObject)
            {
                if (!_isHovering)
                {
                    _isHovering = true;
                    SetColor(hoverColor);

                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayUIHover();
                }
            }
            else if (_isHovering)
            {
                _isHovering = false;
                SetColor(normalColor);
            }
        }
        else
        {
            //Debug.Log("Raycast did not hit anything.");
            if (_isHovering)
            {
                _isHovering = false;
                SetColor(normalColor);
            }
        }
    }

    private void HandleClick()
    {
        if (_isHovering && Input.GetMouseButtonDown(0))
        {
            SetColor(pressedColor);

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayUIClick();

            onClick.Invoke();
        }

        if (_isHovering && Input.GetMouseButtonUp(0))
        {
            SetColor(hoverColor);
        }
    }

    private void SetColor(Color color)
    {
        if (_renderer != null)
            _renderer.material.color = color;
    }
}
