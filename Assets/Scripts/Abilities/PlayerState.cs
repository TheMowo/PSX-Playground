using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public Vector3 Velocity;
    public Vector3 InputDirection;

    public bool IsGrounded;
    public bool IsDashing;
    public bool IsSliding;
    public bool IsCrouching;

    public float CurrentMoveSpeed;
}