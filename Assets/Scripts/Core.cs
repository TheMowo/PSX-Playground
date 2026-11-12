using UnityEngine;

public class Core : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        M_GameManager.Instance.CoreCollected();
        Destroy(gameObject);
    }
}
