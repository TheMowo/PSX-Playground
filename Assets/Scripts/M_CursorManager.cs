using UnityEngine;

public class M_CursorManager : MonoBehaviour
{
    public static M_CursorManager Instance { get; private set; }

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

    private void Start()
    {
        if (M_GameManager.Instance != null)
            HandlePauseStateChanged(M_GameManager.Instance.IsPaused);
    }

    private void OnDisable()
    {
        M_GameManager.PauseStateChanged -= HandlePauseStateChanged;
    }

    private void HandlePauseStateChanged(bool paused)
    {
        //Debug.Log($"Cursor state changed. Paused = {paused}");

        Cursor.lockState =
            paused
                ? CursorLockMode.None
                : CursorLockMode.Locked;

        Cursor.visible = paused;
    }
}