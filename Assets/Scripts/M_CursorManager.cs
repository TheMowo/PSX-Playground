using MoreMountains.Tools;
using UnityEngine;

public class M_CursorManager : MonoBehaviour
{
    M_CursorManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        M_GameManager.PauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        M_GameManager.PauseStateChanged -= HandlePauseStateChanged;
    }

    private void HandlePauseStateChanged(bool paused)
    {
        Cursor.lockState =
            paused
                ? CursorLockMode.None
                : CursorLockMode.Locked;

        Cursor.visible = paused;
    }
}