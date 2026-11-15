
using UnityEngine;

public enum DamageType
{
    Melee,

    BoomerangOutgoing,
    BoomerangReturning,

    Dash,
    Slide,

    Lava,
    Spike,
    Void,

    Enemy
}

public readonly struct DamageInfo
{
    public readonly float Damage;
    public readonly DamageType DamageType;
    public readonly GameObject Source;

    public DamageInfo(float damage, DamageType damageType, GameObject source)
    {
        Damage = damage;
        DamageType = damageType;
        Source = source;
    }
}

public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
}
