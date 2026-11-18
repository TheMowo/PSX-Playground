using UnityEngine;

public sealed class Core : MonoBehaviour
{
    private bool isRegistered;

    private void Start()
    {
        Register();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    private void Register()
    {
        if (isRegistered)
            return;

        if (M_GameManager.Instance == null)
            return;

        M_GameManager.Instance.RegisterCore(this);
        isRegistered = true;
        Debug.Log("Core Registered");
    }

    private void Unregister()
    {
        if (!isRegistered)
            return;

        if (M_GameManager.Instance == null)
            return;

        M_GameManager.Instance.UnregisterCore(this);
        isRegistered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerPublicRef>(out _))
            return;

        M_GameManager.Instance.CoreCollected(this);

        gameObject.SetActive(false);
    }
}