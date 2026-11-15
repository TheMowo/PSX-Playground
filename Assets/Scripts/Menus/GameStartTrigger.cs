using UnityEngine;

public class GameStartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        M_GameManager.Instance.StartGameplay();
    }
}
