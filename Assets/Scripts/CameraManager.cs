using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
            Instance = this;

        }
    }

    private List<CinemachineCamera> cameras = new List<CinemachineCamera>();

    private CinemachineCamera _activeCamera = null;
    public CinemachineCamera ActiveCamera => _activeCamera;

    private CinemachineCamera _playerCam = null;
    public CinemachineCamera PlayerCam => _playerCam;

    public bool IsActiveCamera(CinemachineCamera cam)
    {
        return cam == _activeCamera;
    }

    public void SwitchCamera(CinemachineCamera newCamera)
    {
        newCamera.Priority = 10;
        _activeCamera = newCamera;

        foreach (var cam in cameras)
        {
            if (cam != newCamera) { cam.Priority = 0; }
        }
    }

    public void Register(CinemachineCamera cam)
    {
        cameras.Add(cam);

        if (!_playerCam && cam.CompareTag("PlayerCam"))
        {
            _playerCam = cam;
        }
    }

    public void Unregister(CinemachineCamera cam)
    {
        cameras.Remove(cam);

        if(_playerCam == cam)
        {
            _playerCam = null;
        }
    }
}
