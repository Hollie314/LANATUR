using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AI_Iguane : MonoBehaviour
{
    [Header("Types")]
    // Types
    [SerializeField] private bool is_EatingIguane;
    [SerializeField] private bool is_ShoutingIguane;
    [SerializeField] private bool is_AlphaIguane;
    
    [Header("NoiseRadius")]
    // NoiseRadius
    [SerializeField] private float noiseRadius_Shouting;
    [SerializeField] private float noiseRadius_Running;
    [SerializeField] private float noiseRadius_Walking;
    [SerializeField] private float noiseRadius_Crouching;
    [SerializeField] private MakeNoise _makeNoise;
    
    [Header("Movements")]
    // Movements
    [SerializeField] private float baseSpeed;
    [SerializeField] private float runSpeed;
    
    [Header("Detection")]
    // Movements
    [SerializeField] List<string> ReactAtNoise_Tags = new List<string>();
    
    [Header("EatingIguane variables")]
    // Eating Iguane variables
    [ShowIf("is_EatingIguane")] [SerializeField] private List<Transform> WaypointsBase = new List<Transform>();
    [ShowIf("is_EatingIguane")] [SerializeField] private List<int> WaypointsBaseWithFood = new List<int>();
    [ShowIf("is_EatingIguane")] [SerializeField] private List<Transform> WaypointsOnEvent = new List<Transform>();
    [ShowIf("is_EatingIguane")] [SerializeField] private List<int> WaypointsOnEventWithFood = new List<int>();
    [ShowIf("is_EatingIguane")] [SerializeField] private float TimeToEat;
    [ShowIf("is_EatingIguane")] [SerializeField] private float TimeToRun;
    private float timeEating = 0;
    private float timeRunning = 0;
    private List<Transform> Waypoints;
    private int currentWaypoint = 0;
    
    [Header("ShoutingIguane variables")]
    // Shouting Iguane variables
    [ShowIf("is_ShoutingIguane")] [SerializeField] private GameObject IguaneToProtect;
    [ShowIf("is_ShoutingIguane")] [SerializeField] private AudioClip ShoutAudio;
    [ShowIf("is_ShoutingIguane")] [SerializeField] private float TimeToShout;
    [ShowIf("is_ShoutingIguane")] [SerializeField] private float MaxDistanceFromIguane;
    private float timeShouting = 0;
    
    // Alpha Iguane variables
    [ShowIf("is_AlphaIguane")]
    
    // States
    private bool isEating = false;
    private bool isCarryingFood = false;
    private bool isRunning = false;
    private bool isReleasingFood = false;
    private bool isShouting = false;
    
    // Others
    private Animator animator;
    private AudioSource audioSource;
    private NavMeshAgent agent;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        Waypoints = WaypointsBase;
    }
    
    void OnEnable()
    {
        Digicode.OnCodeEntered += ChangeBehaviour;
    }

    void OnDisable()
    {
        Digicode.OnCodeEntered -= ChangeBehaviour;
    }

    private void ChangeBehaviour(Digicode digicode)
    {
        Waypoints = WaypointsOnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_EatingIguane)
        {
            GoToWaypoint();
            Debug.Log($"isEating: {isEating}");
            if (isEating)
                Eat();
            if (isRunning)
            {
                timeRunning += Time.deltaTime;
                if (timeRunning >= TimeToRun)
                {
                    timeRunning = 0;
                    isRunning = false;
                    agent.speed = baseSpeed;
                }
            }
        }

        if (is_ShoutingIguane)
        {
            if (isShouting)
            {
                timeShouting += Time.deltaTime;
                if (timeShouting >= TimeToShout)
                {
                    isShouting = false;
                    timeShouting = 0;
                    animator.SetBool("IsShouting", false);
                }
            }
            else
            {
                agent.isStopped = false;
                FollowIguane(IguaneToProtect);
            }

            if (isRunning)
            {
                float distanceToIguane = Vector3.Distance(IguaneToProtect.transform.position, transform.position);
                if (distanceToIguane <= MaxDistanceFromIguane + 5)
                {
                    isRunning = false;
                    agent.speed = baseSpeed;
                }
            }
        }
    }

    public void DetectNoise(GameObject detectedObject)
    {
        Debug.Log("Iguane heard noise");
        //vérifier que detectedObject est un type sur lequel l'iguane cris
        if (!ReactAtNoise_Tags.Contains(detectedObject.tag))
            return;
        
        Debug.Log("Iguane recognize noise");
        if (is_ShoutingIguane)
        {
            Debug.Log("Iguane will shout");
            Shout(detectedObject);
            return;
        }

        if (is_EatingIguane)
        {
            Debug.Log("Iguane will run");
            Run();
            return;
        }
    }

    #region EatingIguane
    private void GoToWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(Waypoints[currentWaypoint].position, transform.position);
        Debug.Log($"distanceToWaypoint: {distanceToWaypoint}");

        if (distanceToWaypoint <= 1f && !isEating)
        {
            isEating = true;
            if (Waypoints == WaypointsBase)
            {
                if (WaypointsBaseWithFood.Contains(currentWaypoint))
                    animator.SetBool("IsEating", true);
                else
                    animator.SetBool("IsIdle", true);
                Debug.Log("animator true BaseWP");
            }
            else
            {
                if (WaypointsOnEventWithFood.Contains(currentWaypoint))
                    animator.SetBool("IsEating", true);
                else
                    animator.SetBool("IsIdle", true);
                Debug.Log("animator true OnEventWP");
            }
        }
        
        agent.SetDestination(Waypoints[currentWaypoint].position);
        Debug.Log($"current waypoint {currentWaypoint}");
    }

    private void Eat()
    {
        timeEating += Time.deltaTime;
        Debug.Log("iguane eating");
        if (timeEating >= TimeToEat)
        {
            isEating = false;
            timeEating = 0;
            currentWaypoint = (currentWaypoint + 1) % Waypoints.Count;
            agent.SetDestination(Waypoints[currentWaypoint].position);
            animator.SetBool("IsEating", false);
            animator.SetBool("IsIdle", false);
            Debug.Log("animator faux");
            Debug.Log("changed waypoint");
        }
    }
    
    private void Run()
    {
        Debug.Log("IguaneRun");
        agent.speed = runSpeed;
        isRunning = true;
    }
    #endregion


    #region ShoutingIguane
    private void Shout(GameObject detectedObject)
    {
        Debug.Log("Iguane Shout 1");
        if (isShouting)
            return;
        Debug.Log("Iguane Shout 2");
        float distanceToIguane = Vector3.Distance(IguaneToProtect.transform.position, transform.position);
        if (distanceToIguane > MaxDistanceFromIguane + 5)
        {
            Run();
            return;
        }
        
        Debug.Log("IguaneShout");
        isShouting = true;
        animator.SetBool("IsShouting", true);
        _makeNoise.Noise(this.gameObject, this.transform.position, noiseRadius_Shouting, audioSource, ShoutAudio);
        agent.isStopped = true;
        
        this.transform.LookAt(detectedObject.transform);

        this.GetComponent<ShoutAtTarget>().TryShout(TimeToShout, this.gameObject);
        
        // SphereCast a une certaine distance
        // Si la cible a un rigidbody, repousser avec les méchaniques Rigidbody
        // Si la cible a un CharacterController, repousser avec les méchaniques CharacterController
        // Sinon ne rien faire
    }

    private void FollowIguane(GameObject iguane)
    {
        Debug.Log("Iguane Follows");
        agent.SetDestination(new Vector3(IguaneToProtect.transform.position.x, iguane.transform.position.y + 5, iguane.transform.position.z));
    }
    #endregion
}
