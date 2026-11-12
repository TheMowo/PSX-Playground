using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (M_GameManager.Instance.CanWin)
        {

        }
    }
}
