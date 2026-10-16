using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour // 
{
    /// Navigation System
    // Current Path
    // Current Node
    // Path Recaulculation timer

    /// Locomotion/Movement System
    // Walking
    // Running
    // Jumping
    // Flying
    // Climbing
    // Strafing
    // Sliding
    // Teleporting

    /// Combat Decision System
    // TLDR the brain

    /// Attack Scheduler
    // Needed to prevent overwhelming enemy attacks --> enemies attack in sequence

    //////////////////////////////////////////////////////////////////

    // High Priority:
    // - Dormant until player enters wake-up range
    // - Active --> Chase player (nav agent move)
    // - Health
    // - Speed
    // - Attack Dmg
    // - Attack Coroutine
    // - Attack Range

    // Low priority:
    // - Attack Scheduler

    [Header("Base Stats")]
    private int health = 100;
    private float speed = 15f;
    private int atkDmg = 1;
    private int atkRange = 5;
    private float atkRate = 0.5f;

    [Header("Navigation Setup")]
    [SerializeField] private int stoppingDistance = 4;

    private float pathRefreshRate = 0.15f;
    private float nextPathUpdate;

    [Header("Component References")]
    private EnemyBrain brain;
    private EnemyBlackboard blackboard;
    private EnemyState currentState;

    private NavMeshAgent agent;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        blackboard = GetComponent<EnemyBlackboard>();
        brain = GetComponent<EnemyBrain>();
    }

    private void Start()
    {
        SetupAgent();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Dormant:
                HandleDormant();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

            //case EnemyState.Attack:
            //    HandleAttack();
            //    break;

            //case EnemyState.Dead:
            //    HandleDead();
            //    break;
        }
    }

    private void SetupAgent()
    {
        agent.speed = 8f;
        agent.acceleration = 40f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = stoppingDistance;
        agent.autoTraverseOffMeshLink = true;
    }

    private void HandleDormant()
    {
        agent.isStopped = true;
    }

    private void HandleChase()
    {
        Transform target = PlayerPublicRef.Instance.transform;

        if (Time.time >= nextPathUpdate)
        {
            nextPathUpdate = Time.time + pathRefreshRate;

            agent.SetDestination(blackboard.LastSeenPosition);
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;

            Vector3 lookDir = target.position - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
        else
        {
            agent.isStopped = false;
        }
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
        currentState = state;
    }
}
