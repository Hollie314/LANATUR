using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class IguaneAI : MonoBehaviour
{
    public bool IsAlpha { get; private set; }
    [SerializeField] private float WaitTimeBetweenShouts;
    [SerializeField] private float WaitTimeEating;
    [SerializeField] private float WaitTimeBetweenWaypoints;
    private bool isChasingBaie;
    private bool isEating;
    private bool isShouting;
    [SerializeField] private List<GameObject> ShoutAtTargets;
    [SerializeField] private List<GameObject> GoToTargets;
    private Animator animator;
    private AudioSource audioSource;
    private NavMeshAgent agent;
    private RangeDetector rangeDetector;
    private ShoutAtTarget ShoutAtTarget;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        rangeDetector = GetComponent<RangeDetector>();
        ShoutAtTarget = GetComponent<ShoutAtTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShouting)
        {
            // DetectShoutTargets
            List<GameObject> detected = CheckIfTargetDetected(ShoutAtTargets);
            foreach(GameObject target in detected)
            {
                ShoutAt(target);
            }

            if (detected.Count > 0)
                return;
        }
        
        
        if (!isEating || !isChasingBaie)
        {
            // vérifier qu'il n'y a pas de Baie
            List<GameObject> detected = CheckIfTargetDetected(GoToTargets);
            if (detected.Count > 0)
            {
                GoTo(detected[0]);
                return;
            }
            
            // Continuer de se déplacer en waypoints
        }
        
        
        if (IsAlpha)
        {
            
        }
        else
        {
            
        }
    }

    private List<GameObject> CheckIfTargetDetected(List<GameObject> list)
    {
        List<GameObject> targetsDetected = new List<GameObject>();
        
        List<string> detectedTag = new List<string>();
        foreach (GameObject detected in rangeDetector.GameObjectsDetected)
        {
            detectedTag.Add(detected.tag);
        }
        foreach (GameObject target in list)
        {
            if (detectedTag.Contains(target.tag))
            {
                targetsDetected.Add(target);
            }
        }
        return targetsDetected;
    }

    private void GoTo(GameObject target)
    {
        // animator
    }

    private void ShoutAt(GameObject target)
    {
        Debug.Log("shouting");
        isShouting = true;
        this.transform.LookAt(target.transform);
        // animator
        
        ShoutAtTarget.TryShout(target);
    }

    private void Eat(GameObject target)
    {
        isEating = true;
    }
}
