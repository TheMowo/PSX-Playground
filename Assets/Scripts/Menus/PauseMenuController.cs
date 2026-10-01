using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.Play("Open");
    }

    public void B_ResumeButton()
    {
        animator.Play("Close");
    }

    // Animation Event
    public void ClosePauseMenu()
    {
        M_PauseMenuManager.Instance.ClosePauseMenu();
    }

    public void B_RestartButton()
    {
        M_PauseMenuManager.Instance.ClosePauseMenu();

        M_SceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void B_OptionsButton()
    {

    }

    public void B_BackToMenuButton()
    {
        M_PauseMenuManager.Instance.ClosePauseMenu();

        M_SceneManager.Instance.LoadScene("Main Menu");
    }

    public void B_ExitGame()
    {
        Application.Quit();
    }
}