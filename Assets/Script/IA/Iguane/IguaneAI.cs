using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class IguaneAI : MonoBehaviour
{
    public bool IsAlpha { get; private set; }
    [SerializeField] private List<GameObject> ShoutAtTargets;
    [SerializeField] private List<GameObject> GoToTargets;
    private Animator animator;
    private NavMeshAgent agent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlpha)
        {
            
        }
        else
        {
            
        }
    }

    private void GoTo(GameObject target)
    {
        animator
    }

    private void ShoutAt(GameObject target)
    {
        
    }
}
