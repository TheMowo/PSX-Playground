using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private EnemyBlackboard blackboard;

    [SerializeField] private Transform eyePoint;
    [SerializeField] private LayerMask sightMask;
    [SerializeField] private float sightCheckRate = 0.1f;

    private EnemyController controller;
    //private PlayerPublicRef currentTarget;
    private Collider targetCollider;

    private float waitTime;
    private float nextSightCheck;
    //private float lastSeenTime;

    //private bool hasSeenTarget;

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
        }


        eyePoint ??= transform;

        controller.SetAggro(false);

        waitTime = controller.LoseVisionTimeWindow;
    }

    private void Update()
    {
        if (controller.IsAggro && blackboard.HasSeenTarget && Time.time - blackboard.LastSeenTime > waitTime)
        {
            controller.SetAggro(false);

            blackboard.CurrentTarget = null;
            blackboard.HasSeenTarget = false;

            targetCollider = null;

            return;
        }

        if (blackboard.CurrentTarget == null || Time.time < nextSightCheck)
            return;

        nextSightCheck = Time.time + sightCheckRate;

        CheckLineOfSight();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerPublicRef player))
            return;

        blackboard.CurrentTarget = player.transform;
        targetCollider = other;

        controller.SetAggro(true);

        blackboard.HasSeenTarget = false;

        CheckLineOfSight();
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.TryGetComponent(out PlayerPublicRef player) || player != currentTarget)
    //        return;

    //    // Intentionally left empty.
    //    // Enemy remembers the target until the lose-sight timer expires.
    //}

    private void CheckLineOfSight()
    {
        Vector3 origin = eyePoint.position;
        Vector3 targetPos = targetCollider.bounds.center;
        Vector3 dir = targetPos - origin;

        float distance = dir.magnitude;

        if (distance <= 0.001f)
            return;

        bool canSeePlayer = Physics.Raycast(origin, dir / distance, out RaycastHit hit, distance, sightMask) && hit.collider.TryGetComponent<PlayerPublicRef>(out _);

        Debug.DrawRay(origin, dir.normalized * distance, canSeePlayer ? Color.green : Color.red, sightCheckRate);

        if (canSeePlayer)
        {
            blackboard.LastSeenPosition = targetCollider.bounds.center;
            blackboard.LastSeenTime = Time.time;
        }
        else return;

        blackboard.HasSeenTarget = true;
        blackboard.LastSeenTime = Time.time;
    }
}