using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpVelocity = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterMotor player))
        {
            player.Launch(jumpVelocity);
        }
    }
}