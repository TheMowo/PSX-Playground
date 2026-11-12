using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISoundBinder : MonoBehaviour
{
    [Header("Include inactive objects?")]
    [SerializeField] private bool includeInactive = false;

    private List<Button> buttons = new List<Button>();
    private List<Toggle> toggles = new List<Toggle>();
    private List<Slider> sliders = new List<Slider>();
    private List<Dropdown> dropdowns = new List<Dropdown>();

    //private void Awake()
    //{
    //    // Collect all interactables
    //    buttons.AddRange(GetComponentsInChildren<Button>(includeInactive));
    //    toggles.AddRange(GetComponentsInChildren<Toggle>(includeInactive));
    //    sliders.AddRange(GetComponentsInChildren<Slider>(includeInactive));
    //    dropdowns.AddRange(GetComponentsInChildren<Dropdown>(includeInactive));

    //    // Attach listeners
    //    foreach (var btn in buttons)
    //    {
    //        btn.onClick.AddListener(() =>
    //        {
    //            if (SoundManager.Instance != null)
    //                SoundManager.Instance.PlayUIClick();
    //        });

    //        // Hover requires EventTrigger or pointer events
    //        AddHoverListener(btn.gameObject);
    //    }

    //    foreach (var toggle in toggles)
    //    {
    //        toggle.onValueChanged.AddListener((_) =>
    //        {
    //            if (SoundManager.Instance != null)
    //                SoundManager.Instance.PlayUIClick();
    //        });
    //        AddHoverListener(toggle.gameObject);
    //    }

    //    foreach (var slider in sliders)
    //    {
    //        slider.onValueChanged.AddListener((_) =>
    //        {
    //            if (SoundManager.Instance != null)
    //                SoundManager.Instance.PlayUIClick();
    //        });
    //        AddHoverListener(slider.gameObject);
    //    }

    //    foreach (var dropdown in dropdowns)
    //    {
    //        dropdown.onValueChanged.AddListener((_) =>
    //        {
    //            if (SoundManager.Instance != null)
    //                SoundManager.Instance.PlayUIClick();
    //        });
    //        AddHoverListener(dropdown.gameObject);
    //    }
    //}

    //private void AddHoverListener(GameObject obj)
    //{
    //    var trigger = obj.GetComponent<UnityEngine.EventSystems.EventTrigger>();
    //    if (trigger == null)
    //        trigger = obj.AddComponent<UnityEngine.EventSystems.EventTrigger>();

    //    var entry = new UnityEngine.EventSystems.EventTrigger.Entry
    //    {
    //        eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
    //    };
    //    entry.callback.AddListener((_) =>
    //    {
    //        if (SoundManager.Instance != null)
    //            SoundManager.Instance.PlayUIHover();
    //    });
    //    trigger.triggers.Add(entry);
    //}
}
