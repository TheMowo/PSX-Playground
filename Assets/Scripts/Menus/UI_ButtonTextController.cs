using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ButtonTextController : MonoBehaviour, ISelectHandler, IPointerDownHandler, IDeselectHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color changedColor = Color.green;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>() ?? GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = changedColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        text.color = changedColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = changedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        text.color = defaultColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        text.color = changedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = defaultColor;
    }
}
