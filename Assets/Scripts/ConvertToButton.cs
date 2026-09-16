using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ConvertToButton : MonoBehaviour
{
    [Header("Button Events")]
    [SerializeField] private UnityEvent onClick;

    [Header("Button Colors")]
    [SerializeField] private GameObject buttonVisualObject;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.gray;
    [SerializeField] private Color pressedColor = Color.green;

    private Renderer _renderer;
    private bool _isHovering;

    private void Awake()
    {
        _renderer = buttonVisualObject.GetComponent<Renderer>();
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            if (!_isHovering)
            {
                _isHovering = true;
                SetColor(hoverColor);
            }
        }
        else if (_isHovering)
        {
            _isHovering = false;
            SetColor(normalColor);
        }
    }

    private void HandleClick()
    {
        if (_isHovering && Input.GetMouseButtonDown(0))
        {
            SetColor(pressedColor);
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
