using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    [SerializeField] private LayerMask groundLayers;

    private void Awake()
    {
        if (parentObject == null)
            parentObject = GetComponentInParent<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerPublicRef>(out _))
            return;

        Transform transformation = this.transform;

        if (Physics.Raycast(transformation.position + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 100f, groundLayers))
        {
            transformation.position = hit.point;
        }

        M_CheckpointManager.Instance.SetCheckpoint(transformation);

        Debug.Log($"Checkpoint Activated: {name} at {transformation}");

        Destroy(parentObject);
    }
}