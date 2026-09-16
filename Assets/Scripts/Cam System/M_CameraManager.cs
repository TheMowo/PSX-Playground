using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.Events;

public class M_CameraManager : MonoBehaviour
{
    public static M_CameraManager Instance { get; private set; }

    private readonly List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private CinemachineCamera _activeCamera;
    public CinemachineCamera ActiveCamera => _activeCamera;

    private CinemachineBrain _brain;
    public CinemachineBrain Brain => _brain;

    private const int ActivePriority = 10;
    private const int InactivePriority = 0;

    [Header("Main Menu Cameras")]
    [SerializeField] private CinemachineCamera _settingsCam;
    [SerializeField] private CinemachineCamera _playerCam;
    public CinemachineCamera SettingsCam => _settingsCam;
    public CinemachineCamera PlayerCam => _playerCam;

    // UnityEvent for other scripts to subscribe to
    public UnityEvent<CinemachineCamera> OnBlendFinishedEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _brain = FindAnyObjectByType<CinemachineBrain>();

        var foundCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in foundCams)
        {
            Register(cam);
        }
    }

    private void Start()
    {
        if (_activeCamera == null)
        {
            if (_playerCam != null)
                SwitchCamera(_playerCam);
            else if (cameras.Count > 0)
                SwitchCamera(cameras[0]);
        }
    }

    public bool IsActiveCamera(CinemachineCamera cam) => cam == _activeCamera;

    public void SwitchCamera(CinemachineCamera newCamera)
    {
        if (newCamera == null) return;

        newCamera.Priority = ActivePriority;
        _activeCamera = newCamera;

        foreach (var cam in cameras)
        {
            if (cam != newCamera) cam.Priority = InactivePriority;
        }
    }

    public void Register(CinemachineCamera cam)
    {
        if (cam == null || cameras.Contains(cam)) return;
        cameras.Add(cam);

        if (_playerCam == null && cam.CompareTag("PlayerCam"))
            _playerCam = cam;
    }

    public void Unregister(CinemachineCamera cam)
    {
        if (cam == null) return;
        cameras.Remove(cam);

        if (_playerCam == cam)
            _playerCam = null;
    }

    public void SwitchToSettings() => SwitchCamera(_settingsCam);
    public void SwitchToPlayer() => SwitchCamera(_playerCam);

    // --- Coroutine Helper ---
    public void WaitForBlend(System.Action onComplete)
    {
        StartCoroutine(WaitForBlendRoutine(onComplete));
    }

    private IEnumerator WaitForBlendRoutine(System.Action onComplete)
    {
        // Wait until blending starts
        while (_brain != null && !_brain.IsBlending)
            yield return null;

        // Now wait until blending finishes
        while (_brain != null && _brain.IsBlending)
            yield return null;

        // Blend is complete
        onComplete?.Invoke();
        OnBlendFinishedEvent?.Invoke(_activeCamera);
    }

}
