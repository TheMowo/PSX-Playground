using UnityEngine;

public class M_CheckpointManager : Singleton<M_CheckpointManager>
{
    private Vector3 checkpointPosition;
    private Quaternion checkpointRotation;

    private bool hasCheckpoint;

    public void SetCheckpoint(Transform checkpoint)
    {
        checkpointPosition = checkpoint.position;

        float yRotation = checkpoint.forward.y;
        checkpointRotation = Quaternion.Euler(0f, yRotation, 0f);

        hasCheckpoint = true;
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public Vector3 GetCheckpointPosition()
    {
        return checkpointPosition;
    }

    public Quaternion GetCheckpointRotation()
    {
        return checkpointRotation;
    }
}