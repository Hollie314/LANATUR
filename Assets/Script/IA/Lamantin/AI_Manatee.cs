using UnityEngine;
using UnityEngine.AI;

public class AI_Manatee : MonoBehaviour
{
    public enum State
    {
        Roaming,
        Curious,
        Agitated,
        Panicked,
        GoingToFood,
        Eating,
        GoingToRest,
        Resting,
        Social
    }

    [Header("References")]
    private NavMeshAgent agent;
    private AIBrainMemory memory;
    private MakeNoise makeNoise;

    public AudioSource audioSource;
    public AudioClip lamantinSound;

    [Header("State")]
    public State currentState;

    [Header("Movement Speeds")]
    public float calmSpeed = 2f;
    public float agitatedSpeed = 4f;
    public float panicSpeed = 7f;

    [Header("Stimulus thresholds")]
    public float agitationThreshold = 5f;
    public float panicThreshold = 15f;
    public float lightAttractionThreshold = 8f;

    [Header("Durations")]
    public float eatDuration = 6f;
    public float restDuration = 10f;

    [Header("Noise emission")]
    public float noiseInterval = 5f;
    public float noiseRadius = 20f;
    public float noiseIntensity = 6f;

    private float stateTimer;
    private float noiseTimer;

    private Vector3 homePosition;
    
    [Header("Breaking")]
    public float breakForceCalm = 0.2f;
    public float breakForceAgitated = 0.6f;
    public float breakForcePanicked = 1.0f;
    // ============================
    // INIT
    // ============================
    void OnCollisionEnter(Collision collision)
    {
        TryBreakObject(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        TryBreakObject(other);
    }

    void TryBreakObject(Collider col)
    {
        BreakableObject breakable = col.GetComponent<BreakableObject>();

        if (breakable == null)
            return;

        float force = GetCurrentBreakForce();

        // Probabilité de casser selon force
        if (Random.value <= force)
        {
            breakable.Break();

            //Debug.Log("Lamantin broke object in state: " + currentState);
        }
    }

    float GetCurrentBreakForce()
    {
        switch(currentState)
        {
            case State.Panicked:
                return breakForcePanicked;

            case State.Agitated:
                return breakForceAgitated;

            case State.Roaming:
            case State.Curious:
            case State.GoingToFood:
            case State.GoingToRest:
                return breakForceCalm;

            default:
                return 0.1f;
        }
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        memory = GetComponent<AIBrainMemory>();
        makeNoise = GetComponent<MakeNoise>();

        homePosition = transform.position;
    }
    void Start()
    {
        SetState(State.Roaming);
        GoToRandomPatrolZone();
    }

    void Update()
    {
        HandleState();

        HandleNoiseEmission();
    }

    // ============================
    // STATE MACHINE
    // ============================

    void HandleState()
    {
        switch (currentState)
        {
            case State.Roaming:
                UpdateRoaming();
                break;

            case State.Curious:
                UpdateCurious();
                break;

            case State.Agitated:
                UpdateAgitated();
                break;

            case State.Panicked:
                UpdatePanicked();
                break;

            case State.GoingToFood:
                UpdateGoingToFood();
                break;

            case State.Eating:
                UpdateEating();
                break;

            case State.GoingToRest:
                UpdateGoingToRest();
                break;

            case State.Resting:
                UpdateResting();
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
                agent.speed = calmSpeed;
                break;

            case State.Curious:

                agent.isStopped = false;
                agent.speed = calmSpeed;
                break;

            case State.Agitated:

                agent.isStopped = false;
                agent.speed = agitatedSpeed;

                stateTimer = 5f;
                break;

            case State.Panicked:

                agent.isStopped = false;
                agent.speed = panicSpeed;

                stateTimer = 8f;
                break;

            case State.GoingToFood:

                agent.isStopped = false;
                agent.speed = calmSpeed;

                GoToFoodZone();
                break;

            case State.Eating:

                agent.isStopped = true;

                stateTimer = eatDuration;
                break;

            case State.GoingToRest:

                agent.isStopped = false;
                agent.speed = calmSpeed;

                GoToRestZone();
                break;

            case State.Resting:

                agent.isStopped = true;

                stateTimer = restDuration;
                break;
        }
    }

    // ============================
    // STATE UPDATES
    // ============================

    void UpdateRoaming()
    {
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            GoToRandomPatrolZone();
        }
    }

    void UpdateCurious()
    {
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            SetState(State.Roaming);
        }
    }

    void UpdateAgitated()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            SetState(State.Roaming);
        }
    }

    void UpdatePanicked()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            SetState(State.GoingToFood);
        }
        else if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            FleeRandom();
        }
    }

    void UpdateGoingToFood()
    {
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            SetState(State.Eating);
        }
    }

    void UpdateEating()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            SetState(State.GoingToRest);
        }
    }

    void UpdateGoingToRest()
    {
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            SetState(State.Resting);
        }
    }

    void UpdateResting()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            SetState(State.Roaming);
            GoToRandomPatrolZone();
        }
    }

    // ============================
    // MOVEMENT HELPERS
    // ============================

    void MoveTo(Vector3 position)
    {
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    void GoToFoodZone()
    {
        if (memory == null) return;

        var zone = memory.GetClosestZone(memory.foodZones);

        if (zone != null)
            MoveTo(zone.GetPoint());
    }

    void GoToRestZone()
    {
        if (memory == null) return;

        var zone = memory.GetClosestZone(memory.territoryZones);

        if (zone != null)
            MoveTo(zone.GetPoint());
    }

    void GoToRandomPatrolZone()
    {
        if (memory == null || memory.patrolZones.Count == 0)
        {
            PickRandomDestination();
            return;
        }

        var zone = memory.patrolZones[Random.Range(0, memory.patrolZones.Count)];

        MoveTo(zone.GetPoint());
    }

    void PickRandomDestination()
    {
        Vector3 random = Random.insideUnitSphere * 30f;
        random += homePosition;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(random, out hit, 30f, NavMesh.AllAreas))
        {
            MoveTo(hit.position);
        }
    }

    void FleeRandom()
    {
        Vector3 random = Random.insideUnitSphere * 40f;
        random += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(random, out hit, 40f, NavMesh.AllAreas))
        {
            MoveTo(hit.position);
        }
    }

    void FleeFrom(Vector3 source)
    {
        Vector3 dir = (transform.position - source).normalized;

        Vector3 target = transform.position + dir * 30f;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(target, out hit, 30f, NavMesh.AllAreas))
        {
            MoveTo(hit.position);
        }
    }

    // ============================
    // NOISE
    // ============================

    void HandleNoiseEmission()
    {
        noiseTimer -= Time.deltaTime;

        if (noiseTimer <= 0f)
        {
            EmitNoise();
            noiseTimer = noiseInterval;
        }
    }

    void EmitNoise()
    {
        if (makeNoise == null) return;

        makeNoise.Noise(
            transform.position,
            noiseRadius,
            noiseIntensity,
            gameObject,
            audioSource,
            lamantinSound
        );
    }

    // ============================
    // CALLED BY PERCEPTION
    // ============================

    public void OnAgitated(Vector3 source, float intensity)
    {
        SetState(State.Agitated);

        FleeFrom(source);
    }

    public void OnPanicked(Vector3 source, float intensity)
    {
        SetState(State.Panicked);

        FleeFrom(source);
    }

    public void OnLightAttracted(Vector3 source, float intensity)
    {
        SetState(State.Curious);

        MoveTo(source);
    }
}