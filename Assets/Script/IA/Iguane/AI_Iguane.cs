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
    
    
    // Eating Iguane variables
    [ShowIf("is_eatingIguane")]
    
    // Shouting Iguane variables
    [ShowIf("is_ShoutingIguane")] [SerializeField] private GameObject IguaneToProtect;
    [ShowIf("is_ShoutingIguane")] [SerializeField] private List<Vector3> Waypoints = new List<Vector3>();
    private int currentWaypoint;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        GoToWaypoint();
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
    private void Run()
    {
        Debug.Log("IguaneRun");
    }
    #endregion

    private void GoToWaypoint()
    {
        float distanceToWaypoint = Vector3.Distance(agent.destination, transform.position);

        if (distanceToWaypoint <= 3f)
        {
            currentWaypoint = (currentWaypoint + 1) % Waypoints.Count;
        }
        
        agent.SetDestination(Waypoints[currentWaypoint]);
    }

    #region ShoutingIguane
    private void Shout()
    {
        Debug.Log("IguaneShout");
        // SphereCast a une certaine distance
        // Si la cible a un rigidbody, repousser avec les méchaniques Rigidbody
        // Si la cible a un CharacterController, repousser avec les méchaniques CharacterController
        // Sinon ne rien faire
    }

    private void FollowIguane(GameObject iguane)
    {
        
    }
    #endregion
}
