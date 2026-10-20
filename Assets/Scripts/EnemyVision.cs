using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private Transform eyePoint;
    [SerializeField] private LayerMask sightMask;
    [SerializeField] private float sightCheckRate = 0.1f;

    private EnemyBlackboard blackboard;

    private Collider targetCollider;

    private float nextSightCheck;

    private void Awake()
    {
        blackboard = GetComponentInParent<EnemyBlackboard>();

        if (blackboard == null)
        {
            Debug.LogError($"{name}: EnemyBlackboard not found.");
            enabled = false;
            return;
        }

        eyePoint ??= transform;
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

        targetCollider = other;

        blackboard.CurrentTarget = player.transform;
        blackboard.IsAggro = true;

        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        if (targetCollider == null)
            return;

        if (blackboard.CurrentTarget == null)
            return;

        Vector3 origin = eyePoint.position;
        Vector3 targetPosition = blackboard.CurrentTarget.position;

        Vector3 direction = targetPosition - origin;
        float distance = direction.magnitude;

        bool canSeePlayer = Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, sightMask) && hit.collider.TryGetComponent<PlayerPublicRef>(out _);

        blackboard.CanSeeTarget = canSeePlayer;

        if (canSeePlayer)
        {
            blackboard.HasSeenTarget = true;
            blackboard.IsAggro = true;
        }
    }
}