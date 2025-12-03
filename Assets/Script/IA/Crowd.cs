using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Crowd : MonoBehaviour
{
    public NavMeshAgent NavMeshAgentagent;
    public GameObject Target;
    public GameObject[] AllTargets;

    private void Start()
    {
        //GetComponent<Animator>().SetInteger("Mode", 1);
        FindeTarget(); 
    }

    public void Update()
    {
        if (Target != null)
        {
            if (Vector3.Distance(this.transform.position, Target.transform.position) <= 0.5f)
            {
                FindeTarget();
            }
        }
    }

    public void FindeTarget()
    {
        if (Target != null)
            Target.transform.tag = "Target";
        
        AllTargets = GameObject.FindGameObjectsWithTag("Target");
        Target = AllTargets[Random.Range(0, AllTargets.Length)];
        Target.transform.tag = "Target";
        
        NavMeshAgentagent.SetDestination(Target.transform.position);
    }
}
