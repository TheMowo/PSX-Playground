using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelectionKeeper : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelection;

    private GameObject lastSelectedObject;

    private void Start()
    {
        if (EventSystem.current == null)
            return;

        if (defaultSelection != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelection);
            lastSelectedObject = defaultSelection;
        }
    }

    private void Update()
    {
        if (EventSystem.current == null)
            return;

        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current != null)
        {
            lastSelectedObject = current;
        }

        bool usingMouse =
            M_InputManager.Instance != null &&
            M_InputManager.Instance.UsingMouse;

        if (!usingMouse && current == null)
        {
            EventSystem.current.SetSelectedGameObject(
                lastSelectedObject != null
                    ? lastSelectedObject
                    : defaultSelection);
        }
    }
}