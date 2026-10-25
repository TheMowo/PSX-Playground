using UnityEngine;

public class M_CheckpointManager : Singleton<M_CheckpointManager>
{
    private Vector3 checkpointPosition;
    private bool hasCheckpoint;

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        hasCheckpoint = true;

        Debug.Log($"Checkpoint set: {position}");
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public Vector3 GetCheckpointPosition()
    {
        return checkpointPosition;
    }
}