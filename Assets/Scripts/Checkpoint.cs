using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayers;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerPublicRef>(out _))
            return;

        float playerHeight = 2f;

        if (CharacterMotor.Instance &&
            CharacterMotor.Instance.TryGetComponent(out CharacterController controller))
        {
            playerHeight = controller.height;
        }

        Vector3 respawnPosition = transform.position;

        if (Physics.Raycast(respawnPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 100f, groundLayers))
        {
            respawnPosition = hit.point + Vector3.up * (playerHeight * 0.5f);
        }

        M_CheckpointManager.Instance.SetCheckpoint(respawnPosition);

        Debug.Log($"Checkpoint Activated: {respawnPosition}");

        Destroy(gameObject);
    }
}