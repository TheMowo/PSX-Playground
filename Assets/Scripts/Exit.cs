using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.TryGetComponent<PlayerPublicRef>(out PlayerPublicRef player))
            return;

        if (M_GameManager.Instance.CanWin)
        {
            M_ScoreManager.Instance.AddScore(ScoreId.Exit);
            M_GameManager.Instance.GameplayEnd();
        }
    }
}
