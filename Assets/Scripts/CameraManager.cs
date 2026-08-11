using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private readonly List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private CinemachineCamera _activeCamera;
    public CinemachineCamera ActiveCamera => _activeCamera;

    private CinemachineCamera _playerCam;
    public CinemachineCamera PlayerCam => _playerCam;

    private const int ActivePriority = 10;
    private const int InactivePriority = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // Auto-discover all cameras in the scene
        var foundCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        foreach (var cam in foundCams)
        {
            Register(cam);
        }
    }

    private void Start()
    {
        // Ensure we have an active camera before gameplay
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
        {
            _playerCam = cam;
        }
    }

    public void Unregister(CinemachineCamera cam)
    {
        if (cam == null) return;

        cameras.Remove(cam);

        if (_playerCam == cam)
        {
            _playerCam = null;
        }
    }
}
