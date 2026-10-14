using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private EnemyBlackboard blackboard;

    [SerializeField] private Transform eyePoint;
    [SerializeField] private LayerMask sightMask;
    [SerializeField] private float sightCheckRate = 0.1f;

    private EnemyController controller;
    private Collider targetCollider;

    private float nextSightCheck;

    private void Awake()
    {
        controller = GetComponentInParent<EnemyController>();

        if (controller == null)
        {
            Debug.LogError($"{name}: EnemyController not found.");
            enabled = false;
            return;
        }

        blackboard = GetComponentInParent<EnemyBlackboard>();

        if (blackboard == null)
        {
            Debug.LogError($"{name}: EnemyBlackBoard not found.");
            enabled = false;
            return;
        }

        eyePoint ??= transform;

        blackboard.CanSeeTarget = false;
    }

    private void Update()
    {
        if (blackboard.CurrentTarget == null)
            return;

        if (Time.time < nextSightCheck)
            return;

        nextSightCheck = Time.time + sightCheckRate;

        CheckLineOfSight();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerPublicRef player))
            return;

        Debug.Log($"EnemyVision: Detected {other.name}");

        blackboard.CurrentTarget = player.transform;
        targetCollider = other;

        // Enemy aggroes immediately when player enters detection range.
        blackboard.IsAggro = true;

        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        if (targetCollider == null)
            return;

        Vector3 origin = eyePoint.position;
        Vector3 targetPos = blackboard.CurrentTarget.position;

        Vector3 dir = targetPos - origin;

        float distance = dir.magnitude;

        if (distance <= 0.001f)
            return;

        bool canSeePlayer = Physics.Raycast(origin, dir / distance, out RaycastHit hit, distance, sightMask) && hit.collider.TryGetComponent<PlayerPublicRef>(out _);

        Debug.DrawRay(origin, dir.normalized * distance, canSeePlayer ? Color.green : Color.red, sightCheckRate);

        blackboard.CanSeeTarget = canSeePlayer;

        if (!canSeePlayer)
            return;

        // Permanent aggro once LOS is achieved.
        blackboard.IsAggro = true;

        blackboard.HasSeenTarget = true;

        blackboard.LastSeenPosition =
            blackboard.CurrentTarget.position;
    }
}