using System;
using UnityEngine;

public enum EnemyState
{
    Dormant,
    Chase,
    Attack,
    Stunned,
    Dead
}

public class EnemyBrain : MonoBehaviour
{
    public event Action<EnemyState> OnStateChanged;

    [SerializeField] private float attackRange = 3f;

    private EnemyBlackboard blackboard;
    private EnemyState currentState;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        blackboard = GetComponent<EnemyBlackboard>();

        blackboard.IsAlive = true;
        blackboard.IsAggro = false;
    }

    private void Update()
    {
        EvaluateState();
    }

    private void EvaluateState()
    {
        blackboard.WantsToAttack = false;

        if (!blackboard.IsAlive)
        {
            SetState(EnemyState.Dead);
            return;
        }

        if (!blackboard.IsAggro)
        {
            SetState(EnemyState.Dormant);
            return;
        }

        if (blackboard.CurrentTarget == null)
        {
            SetState(EnemyState.Dormant);
            return;
        }

        if (blackboard.IsAttacking)
        {
            SetState(EnemyState.Attack);
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            blackboard.LastSeenPosition);

        bool canAttack =
            distance <= attackRange &&
            !blackboard.AttackOnCooldown;

        if (canAttack)
        {
            SetState(EnemyState.Attack);
            return;
        }

        SetState(EnemyState.Chase);
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log($"{name} -> {currentState}");

        OnStateChanged?.Invoke(currentState);
    }
}