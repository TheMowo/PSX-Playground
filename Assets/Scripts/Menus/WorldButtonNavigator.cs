using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldButtonNavigator : MonoBehaviour
{
    public static WorldButtonNavigator Instance { get; private set; }

    [Header("Starting Selection")]
    [SerializeField] private ConvertToButton startingButton;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;

    [Header("Navigation")]
    [SerializeField] private float navigationDeadzone = 0.5f;

    private ConvertToButton currentButton;
    private bool navigationReleased = true;


    // Custom shitttt
    private List<ConvertToButton> menuButtons = new List<ConvertToButton>();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Debug.Log("Navigator Enabled");

        navigateAction.action.Enable();
        submitAction.action.Enable();

        if (startingButton != null)
        {
            SelectButton(startingButton);
        }

        SelectButton(startingButton);
    }

    private void OnDisable()
    {
        navigateAction?.action.Disable();
        submitAction?.action.Disable();
    }

    private void Update()
    {
        HandleNavigation();
        HandleSubmit();
        Debug.Log(currentButton.gameObject.name);
    }

    private void HandleNavigation()
    {
        if (navigateAction == null)
            return;

        Vector2 input = navigateAction.action.ReadValue<Vector2>();

        Debug.Log($"Navigation Input: {input}");

        if (!navigationReleased)
        {
            if (Mathf.Abs(input.x) < 0.2f &&
                Mathf.Abs(input.y) < 0.2f)
            {
                navigationReleased = true;
            }

            return;
        }

        ConvertToButton target = null;

        if (input.y > navigationDeadzone)
        {
            target = currentButton?.up;
        }
        else if (input.y < -navigationDeadzone)
        {
            target = currentButton?.down;
        }
        else if (input.x > navigationDeadzone)
        {
            target = currentButton?.right;
        }
        else if (input.x < -navigationDeadzone)
        {
            target = currentButton?.left;
        }

        if (target != null)
        {
            SelectButton(target);
            navigationReleased = false;
        }
    }

    private void HandleSubmit()
    {
        if (currentButton == null)
            return;

        bool pressed = false;
        bool released = false;

        if (submitAction != null)
        {
            pressed = submitAction.action.WasPressedThisFrame();
            released = submitAction.action.WasReleasedThisFrame();
        }

        if (Mouse.current != null)
        {
            pressed |= Mouse.current.leftButton.wasPressedThisFrame;
            released |= Mouse.current.leftButton.wasReleasedThisFrame;
        }

        if (pressed)
        {
            currentButton.HandlePress();
        }

        if (released)
        {
            currentButton.HandleRelease();
        }
    }

    public void SelectButton(ConvertToButton button)
    {
        if (button == currentButton)
            return;

        currentButton?.HandleDeselect();

        currentButton = button;

        currentButton?.HandleSelect();
    }

    public ConvertToButton GetCurrentSelection()
    {
        return currentButton;
    }
}