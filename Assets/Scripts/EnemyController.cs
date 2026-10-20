using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private float pathRefreshRate = 0.15f;
    [SerializeField] private float stoppingDistance = 4f;

    private EnemyBrain brain;
    private EnemyBlackboard blackboard;
    private NavMeshAgent agent;

    private EnemyState currentState;

    private float nextPathUpdate;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();
        blackboard = GetComponent<EnemyBlackboard>();
        agent = GetComponent<NavMeshAgent>();
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
        if (blackboard.CurrentTarget == null)
            return;

        if (Time.time >= nextPathUpdate)
        {
            nextPathUpdate = Time.time + pathRefreshRate;

            blackboard.CurrentDestination =
                blackboard.CurrentTarget.position;

            agent.SetDestination(
                blackboard.CurrentDestination);
        }

        agent.isStopped = false;
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