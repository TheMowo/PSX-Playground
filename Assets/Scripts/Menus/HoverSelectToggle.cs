using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HoverSelectToggle : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerDownHandler, ISubmitHandler
{
    private Toggle toggle;
    private M_MenuPageManager pageManager;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        pageManager = GetComponentInParent<M_MenuPageManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);

        PreviewTab();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PreviewTab();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ActivateTab();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        ActivateTab();
    }

    private void PreviewTab()
    {
        if (pageManager == null)
            return;

        int pageIndex = pageManager.GetTabIndex(toggle);

        if (pageIndex >= 0)
            pageManager.PreviewPage(pageIndex);
    }

    private void ActivateTab()
    {
        if (toggle == null)
            return;

        toggle.isOn = true; // Fires OnValueChanged
    }
}