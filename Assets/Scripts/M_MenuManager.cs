using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class M_MenuManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Button_CloseSettings();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Button_OpenSettings();
        }
    }

    [Header("Current Index")]
    [ShowInInspector] [Sirenix.OdinInspector.ReadOnly] private int _pageIndex = 0;

    [Header("Components")]
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Animator _animator;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private List<Toggle> _tabs = new List<Toggle>();
    [SerializeField] private List<CanvasGroup> _pages = new List<CanvasGroup>();

    [Header("Event to call")]
    public UnityEvent<int> OnPageIndexChanged;

    private void Initialize()
    {
        _toggleGroup = GetComponentInChildren<ToggleGroup>(true);

        _tabs.Clear();
        _pages.Clear();

        _tabs.AddRange(GetComponentsInChildren<Toggle>(true));
        _tabs.RemoveAll(t => !t.CompareTag("ToggleButton"));
        _pages.AddRange(GetComponentsInChildren<CanvasGroup>(true));
    }

    private void Reset()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        if (_tabs.Count > 0)
            _tabs[_pageIndex].SetIsOnWithoutNotify(true);
    }



    private void Awake()
    {
        Initialize();
        foreach (var toggle in _tabs)
        {
            toggle.onValueChanged.AddListener(CheckForTab);
            toggle.group = _toggleGroup;
        }

        if (_settingsMenu != null)
            _settingsMenu.SetActive(false);

        _pageIndex = 0;
        JumpToPage(_pageIndex);
    }


    private void OnDestroy()
    {
        foreach (var toggle in _tabs)
        {
            toggle.onValueChanged.RemoveListener(CheckForTab);
        }
    }

    private void CheckForTab(bool value)
    {
        for (int i = 0; i < _tabs.Count; i++)
        {
            if (!_tabs[i].isOn) continue;
            _pageIndex = i;
        }

        OpenPage(_pageIndex);
    }

    private void OpenPage(int index)
    {
        EnsureIndexIsInRange(index);

        for (int i = 0; i < _tabs.Count; i++)
        {
            bool isActivePage = (i == index);

            _pages[i].alpha = isActivePage ? 1 : 0;
            _pages[i].interactable = isActivePage;
            _pages[i].blocksRaycasts = isActivePage;
        }

        if (Application.isPlaying) OnPageIndexChanged?.Invoke(_pageIndex);
    }

    private void EnsureIndexIsInRange(int index)
    {
        if (_tabs.Count == 0 || _pages.Count == 0)
        {
            Debug.LogWarning("Forgot to setup Tabs or Pages");
            return;
        }
        _pageIndex = Mathf.Clamp(index, 0, _pages.Count - 1);
    }


    public void JumpToPage(int page)
    {
        if (_tabs == null || _tabs.Count == 0)
        {
            Debug.LogWarning("JumpToPage called but no tabs are set up.");
            return;
        }

        EnsureIndexIsInRange(page);

        if (_pageIndex >= 0 && _pageIndex < _tabs.Count)
        {
            _tabs[_pageIndex].isOn = true;
        }
    }

    // --- Button methods ---
    public void Button_NewGame() => Debug.Log("Pressed New Game Button");
    public void Button_Continue() => Debug.Log("Pressed Continue Button");
    public void Button_Exit() => Debug.Log("Pressed Exit Button");

    public void Button_OpenSettings()
    {
        M_CameraManager.Instance.SwitchToSettings();

        M_CameraManager.Instance.WaitForBlend(() =>
        {
            Debug.Log("Blend complete!");
            if (_settingsMenu != null)
            {
                _settingsMenu.SetActive(true);
                Initialize();
            }
        });
    }

    public void Button_CloseSettings()
    {
        if (_settingsMenu != null)
            _animator.SetTrigger("close");

        M_CameraManager.Instance.SwitchToPlayer();
    }
}
