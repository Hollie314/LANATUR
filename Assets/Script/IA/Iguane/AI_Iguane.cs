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
    [SerializeField] private float noiseRadius_Running;
    [SerializeField] private float noiseRadius_Walking;
    [SerializeField] private float noiseRadius_Crouching;
    [SerializeField] private MakeNoise _makeNoise;
    
    [Header("Movements")]
    // Movements
    [SerializeField] private float baseSpeed;
    [SerializeField] private float runSpeed;
    
    [Header("EatingIguane variables")]
    // Eating Iguane variables
    [ShowIf("is_EatingIguane")] [SerializeField] private List<Transform> WaypointsBase = new List<Transform>();
    [ShowIf("is_EatingIguane")] [SerializeField] private List<Transform> WaypointsOnEvent = new List<Transform>();
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
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        Waypoints = WaypointsBase;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_EatingIguane)
        {
            GoToWaypoint();
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
                    animator.SetBool("isShouting", false);
                }
            }
            else
                FollowIguane(IguaneToProtect);
        }
    }

    public void DetectNoise(GameObject detectedObject)
    {
        //vérifier que detectedObject est un type sur lequel l'iguane cris
        if (is_ShoutingIguane)
        {
            Shout();
            return;
        }

        if (is_EatingIguane)
        {
            Run();
            return;
        }
    }

    #region EatingIguane
    private void GoToWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(agent.destination, transform.position);

        if (distanceToWaypoint <= 0.5f && !isEating)
        {
            isEating = true;
        }
        
        agent.SetDestination(Waypoints[currentWaypoint].position);
    }

    private void Eat()
    {
        timeEating += Time.deltaTime;
        if (timeEating >= TimeToEat)
        {
            isEating = false;
            timeEating = 0;
            currentWaypoint = (currentWaypoint + 1) % Waypoints.Count;
            Debug.Log($"current waypoint {currentWaypoint}");
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
    private void Shout()
    {
        if (isShouting)
            return;
        float distanceToIguane = Vector3.Distance(IguaneToProtect.transform.position, transform.position);
        if (distanceToIguane > MaxDistanceFromIguane)
            return;
        
        Debug.Log("IguaneShout");
        isShouting = true;
        animator.SetBool("isShouting", true);
        audioSource.clip = ShoutAudio;
        audioSource.Play();
        // SphereCast a une certaine distance
        // Si la cible a un rigidbody, repousser avec les méchaniques Rigidbody
        // Si la cible a un CharacterController, repousser avec les méchaniques CharacterController
        // Sinon ne rien faire
    }

    private void FollowIguane(GameObject iguane)
    {
        agent.SetDestination(new Vector3(IguaneToProtect.transform.position.x + 2, iguane.transform.position.y, iguane.transform.position.z));
    }
    #endregion
}
