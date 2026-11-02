using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerRespawn : MonoBehaviour
{
    private PlayerHealth health;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        health.OnDeath += RespawnAtSpawn;
    }

    private void OnDisable()
    {
        health.OnDeath -= RespawnAtSpawn;
    }

    private void RespawnAtSpawn()
    {
        Vector3 spawnPosition = M_SpawnManager.Instance.GetSpawnPosition();
        Quaternion spawnRotation = M_SpawnManager.Instance.GetSpawnRotation();

        transform.position = spawnPosition;

        if (TryGetComponent(out CharacterMotor motor))
            motor.ResetMovementState();

        Debug.Log($"Respawning to Global Spawnpoint at {spawnPosition}");
    }

    public void RespawnAtCheckpoint()
    {
        Vector3 checkpointPosition = M_CheckpointManager.Instance.GetCheckpointPosition();
        transform.SetPositionAndRotation(checkpointPosition, M_CheckpointManager.Instance.GetCheckpointRotation());

        if (TryGetComponent(out CharacterMotor motor))
            motor.ResetMovementState();

        Debug.Log($"Respawning to Checkpoint at {checkpointPosition}");
    }
}