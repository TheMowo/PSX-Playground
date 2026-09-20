using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    protected PlayerBrain brain;
    protected PlayerState state;

    public virtual int Priority => 0;

    public virtual void Initialize(PlayerBrain owner)
    {
        brain = owner;
        state = owner.State;
    }

    public virtual void Tick() { }

    public virtual void LateTick() { }
}