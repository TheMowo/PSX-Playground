using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class ConvertToButton : MonoBehaviour
{
    [Header("Navigation")]
    public ConvertToButton up;
    public ConvertToButton down;
    public ConvertToButton left;
    public ConvertToButton right;

    [SerializeField] private RenderTexture rentex;

    [Header("Button Events")]
    [SerializeField] private UnityEvent onClick;

    [Header("Button Colors")]
    [SerializeField] private GameObject buttonVisualObject;
    [SerializeField] private string normalHex = "#525252";
    [SerializeField] private string hoverHex = "#808080";
    [SerializeField] private string pressedHex = "#00FF00";

    private Color normalColor;
    private Color hoverColor;
    private Color pressedColor;

    private Renderer cachedRenderer;

    public bool IsHovered { get; private set; }

    private void Awake()
    {
        cachedRenderer = buttonVisualObject.GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(normalHex, out normalColor);
        ColorUtility.TryParseHtmlString(hoverHex, out hoverColor);
        ColorUtility.TryParseHtmlString(pressedHex, out pressedColor);

        SetColor(normalColor);
    }

    private void Update()
    {
        HandlePointerHover();
    }

    private void HandlePointerHover()
    {
        if (M_MenuCam.Instance == null)
            return;

        Vector2 pointerPosition = GetPointerPosition();

        Ray ray = M_MenuCam.Instance.GetActiveCameraRay(pointerPosition, rentex);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!IsHovered)
                {
                    WorldButtonNavigator.Instance?.SelectButton(this);
                }
            }
        }
    }

    private Vector2 GetPointerPosition()
    {
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        return Vector2.zero;
    }

    public void HandleSelect()
    {
        if (IsHovered)
            return;

        IsHovered = true;

        SetColor(hoverColor);

        //SoundManager.Instance?.PlayUIHover();
    }

    public void HandleDeselect()
    {
        if (!IsHovered)
            return;

        IsHovered = false;

        SetColor(normalColor);
    }

    public void HandlePress()
    {
        SetColor(pressedColor);

        //SoundManager.Instance?.PlayUIClick();

        onClick?.Invoke();
    }

    public void HandleRelease()
    {
        if (IsHovered)
            SetColor(hoverColor);
        else
            SetColor(normalColor);
    }

    private void SetColor(Color color)
    {
        if (cachedRenderer != null)
            cachedRenderer.material.color = color;
    }
}