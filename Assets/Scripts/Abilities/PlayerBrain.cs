using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class PlayerBrain : MonoBehaviour
{
    public PlayerState State { get; private set; }

    public CharacterController Controller { get; private set; }
    public Animator Animator { get; private set; }

    private PlayerAbility[] abilities;

    void Awake()
    {
        State = new PlayerState();

        Controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();

        abilities = GetComponents<PlayerAbility>()
            .OrderBy(x => x.Priority)
            .ToArray();

        foreach (PlayerAbility ability in abilities)
            ability.Initialize(this);
    }

    void Update()
    {
        State.IsGrounded = Controller.isGrounded;

        foreach (PlayerAbility ability in abilities)
        {
            if (ability.enabled)
                ability.Tick();
        }

        Controller.Move(State.Velocity * Time.deltaTime);

        foreach (PlayerAbility ability in abilities)
        {
            if (ability.enabled)
                ability.LateTick();
        }
    }
}