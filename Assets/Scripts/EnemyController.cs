using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour // 
{
    // State Machine


    /// Perception System
    // Vision
    // Player Activity (movement, gun firing, etc.)

    // Memory:
    // Last Player Position
    // Last Seen Time
    // Threat Level

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


    /// Attack Scheduler
    // 

    /// Positining System

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

    private int currentHealth = 1;

    private bool isAggro;

    [Header("Navigation Setup")]
    private int stoppingDistance = 4;
    private float loseVisionTimeWindow = 2;

    private float pathRefreshRate = 0.15f;
    private float nextPathUpdate;

    [Header("Component References")]
    private EnemyBlackboard blackboard;
    private NavMeshAgent agent;

    [Header("Public References")]
    public float LoseVisionTimeWindow => loseVisionTimeWindow;
    public bool IsAggro => isAggro;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHealth = health;
    }

    private void Start()
    {
        SetupAgent();
    }

    private void FixedUpdate()
    {
        if (isAggro)
            ChasePlayer();

    }

    private void SetupAgent()
    {
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
        agent.autoTraverseOffMeshLink = true;
    }

    private void ChasePlayer()
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

    private void InitAttack()
    {

    }

    public void SetAggro(bool aggro)
    {
        Debug.Log($"Enemy {gameObject.name} isAggro set to {aggro}");
        isAggro = aggro;
    }
}
