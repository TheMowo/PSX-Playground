using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class M_MenuPageManager : MonoBehaviour
{
    [Header("Page Index")]
    [SerializeField] private int pageIndex;

    [Header("Components")]
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private List<Toggle> tabs = new();
    [SerializeField] private List<CanvasGroup> pages = new();

    private void Awake()
    {
        AssignToggleGroup();
        ApplyPageState();
    }

    private void OnEnable()
    {
        AssignToggleGroup();
        ApplyPageState();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AssignToggleGroup();
        ApplyPageState();
    }
#endif

    private void AssignToggleGroup()
    {
        if (toggleGroup == null)
            return;

        foreach (Toggle tab in tabs)
        {
            if (tab != null)
            {
                tab.group = toggleGroup;
            }
        }
    }

    public int GetTabIndex(Toggle toggle)
    {
        return tabs.IndexOf(toggle);
    }

    public void SetPage(int index)
    {
        if (pages == null || pages.Count == 0)
            return;

        pageIndex = Mathf.Clamp(index, 0, pages.Count - 1);

        ApplyPageState();
    }

    public void NextPage()
    {
        SetPage(pageIndex + 1);
    }

    public void PreviousPage()
    {
        SetPage(pageIndex - 1);
    }

    private void ApplyPageState()
    {
        if (pages == null || pages.Count == 0)
            return;

        pageIndex = Mathf.Clamp(pageIndex, 0, pages.Count - 1);

        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] == null)
                continue;

            bool active = i == pageIndex;

            pages[i].alpha = active ? 1f : 0f;
            pages[i].interactable = active;
            pages[i].blocksRaycasts = active;
        }

        bool usingMouse =
            M_InputManager.Instance != null &&
            M_InputManager.Instance.UsingMouse;

        if (!usingMouse &&
            EventSystem.current != null &&
            pageIndex < tabs.Count &&
            tabs[pageIndex] != null &&
            EventSystem.current.currentSelectedGameObject != tabs[pageIndex].gameObject)
        {
            EventSystem.current.SetSelectedGameObject(
                tabs[pageIndex].gameObject);
        }
    }
    public void PreviewPage(int index)
    {
        if (pages == null || pages.Count == 0)
            return;

        pageIndex = Mathf.Clamp(index, 0, pages.Count - 1);

        ApplyPageState();
    }
}