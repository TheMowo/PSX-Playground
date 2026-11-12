using UnityEngine;

public class M_PauseMenuManager : Singleton<M_PauseMenuManager>
{
    [SerializeField] private GameObject pauseMenu;
    private void Start()
    {
        if (M_InputManager.Instance != null)
            M_InputManager.Instance.PausePressed += HandlePauseInput;
    }

    private void OnDestroy()
    {
        if (M_InputManager.Instance != null)
            M_InputManager.Instance.PausePressed -= HandlePauseInput;
    }

    private void HandlePauseInput()
    {
        if (M_GameManager.Instance.HasPauseReason(M_GameManager.PauseReason.Dialogue))
        {
            //Debug.Log("Pause attempted, but PauseReason.Dialogue was present.");
            return;
        }

        if (M_GameManager.Instance.HasPauseReason(M_GameManager.PauseReason.PauseMenu))
        {
            //Debug.Log("Pause Menu Close prompted");
            ClosePauseMenu();
        }
        else
        {
            //Debug.Log("Pause Menu Open prompted");
            OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);

        M_GameManager.Instance.AddPauseReason(M_GameManager.PauseReason.PauseMenu);
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);

        M_GameManager.Instance.RemovePauseReason(M_GameManager.PauseReason.PauseMenu);
    }
}