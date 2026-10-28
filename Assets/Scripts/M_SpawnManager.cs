using UnityEngine;

public class M_SpawnManager : Singleton<M_SpawnManager>
{
    [SerializeField] private Transform defaultSpawn;

    public Vector3 GetSpawnPosition()
    {
        if (M_CheckpointManager.Instance.HasCheckpoint())
        {
            return M_CheckpointManager.Instance.GetCheckpointPosition();
        }

        return defaultSpawn.position;
    }
}