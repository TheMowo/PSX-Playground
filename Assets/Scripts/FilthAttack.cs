using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FilthAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private GameObject attackHitbox;

    [Header("Lunge")]
    [SerializeField] private float lungeForce = 12f;
    [SerializeField] private float lungeUpwardForce = 2f;

    [Header("Timing")]
    [SerializeField] private float windupTime = 0.25f;
    [SerializeField] private float recoveryTime = 0.5f;

    private EnemyBrain brain;
    private EnemyBlackboard blackboard;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();
        blackboard = GetComponent<EnemyBlackboard>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        brain.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        brain.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(EnemyState state)
    {
        if (state != EnemyState.Attack)
            return;

        if (blackboard.IsAttacking)
            return;

        if (blackboard.AttackOnCooldown)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("FILTH ATTACK START");

        blackboard.IsAttacking = true;
        blackboard.AttackOnCooldown = true;
        blackboard.HasDealtAttackDamage = false;

        agent.isStopped = true;

        Vector3 targetPosition = blackboard.LastSeenPosition;

        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(lookDirection);

        yield return new WaitForSeconds(windupTime);

        attackHitbox.SetActive(true);

        agent.enabled = false;

        rb.linearVelocity = Vector3.zero;

        Vector3 launchDirection =
            (targetPosition - transform.position).normalized;

        rb.linearVelocity =
            launchDirection * lungeForce +
            Vector3.up * lungeUpwardForce;

        yield return new WaitForSeconds(0.15f);

        attackHitbox.SetActive(false);

        yield return new WaitForSeconds(recoveryTime);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        agent.enabled = true;
        agent.Warp(transform.position);
        agent.isStopped = false;

        blackboard.IsAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        blackboard.AttackOnCooldown = false;
    }
}