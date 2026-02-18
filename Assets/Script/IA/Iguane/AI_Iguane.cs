using UnityEngine;
using UnityEngine.AI;

public class AI_Iguane : MonoBehaviour
{
    public enum State
    {
        Roaming,
        GoingToFood,
        CarryingFood,
        Fleeing,
        HidingInNest,
        Resting
    }

    [Header("References")]
    private NavMeshAgent agent;
    private AIBrainMemory memory;

    [Header("State")]
    public State currentState;

    [Header("Movement Speeds")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Noise thresholds")]
    public float fleeThreshold = 5f;
    public float hideThreshold = 15f;

    [Header("Timers")]
    public float restDuration = 5f;
    private float stateTimer;

    [Header("Waypoints & Nest")]
    public Transform nest;
    private Transform targetFood;

    private Vector3 homePosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        memory = GetComponent<AIBrainMemory>();
        homePosition = transform.position;

        // Autorise navigation dans l'eau si nécessaire
        agent.areaMask |= 1 << NavMesh.GetAreaFromName("Water"); 
    }

    void Start()
    {
        SetState(State.Roaming);
        PickRandomFood();
    }

    void Update()
    {
        HandleState();
    }

    // ==========================
    // STATE MACHINE
    // ==========================
    void HandleState()
    {
        switch (currentState)
        {
            case State.Roaming:
                if (!agent.pathPending && agent.remainingDistance < 1f)
                    PickRandomFood();
                break;

            case State.GoingToFood:
                if (!agent.pathPending && agent.remainingDistance < 1f)
                    SetState(State.CarryingFood);
                break;

            case State.CarryingFood:
                if (!agent.pathPending && agent.remainingDistance < 1f)
                    SetState(State.Resting);
                break;

            case State.Fleeing:
            case State.HidingInNest:
            case State.Resting:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    SetState(State.Roaming);
                    PickRandomFood();
                }
                break;
        }
    }

    void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Roaming:
                agent.isStopped = false;
                agent.speed = walkSpeed;
                break;

            case State.GoingToFood:
                agent.isStopped = false;
                agent.speed = walkSpeed;
                if (targetFood != null)
                    agent.SetDestination(targetFood.position);
                break;

            case State.CarryingFood:
                agent.isStopped = false;
                agent.speed = runSpeed;
                if (nest != null)
                    agent.SetDestination(nest.position);
                break;

            case State.Fleeing:
                agent.isStopped = false;
                agent.speed = runSpeed;
                FleeRandomDirection();
                stateTimer = 3f; // temps à fuir avant retourner
                break;

            case State.HidingInNest:
                agent.isStopped = false;
                agent.speed = runSpeed;
                if (nest != null)
                    agent.SetDestination(nest.position);
                stateTimer = 5f;
                break;

            case State.Resting:
                agent.isStopped = true;
                stateTimer = restDuration;
                break;
        }
    }

    // ==========================
    // HELPERS
    // ==========================
    void PickRandomFood()
    {
        if (memory.foodZones.Count == 0)
            return;

        AIZone zone = memory.GetClosestZone(memory.foodZones);
        if (zone != null)
        {
            targetFood = zone.transform;
            SetState(State.GoingToFood);
        }
    }

    void FleeRandomDirection()
    {
        Vector3 randomDir = Random.insideUnitSphere * 10f;
        randomDir += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, 10f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    // ==========================
    // CALLED BY PERCEPTION
    // ==========================
    public void OnNoise(Vector3 sourcePosition, float intensity)
    {
        float distance = Vector3.Distance(transform.position, sourcePosition);
        if (distance > intensity) return;

        if (intensity >= hideThreshold)
            SetState(State.HidingInNest);
        else if (intensity >= fleeThreshold)
            SetState(State.Fleeing);
    }
}