using UnityEngine;
using System.Collections;

public class Active_gravity : MonoBehaviour
{
    [SerializeField] private Rigidbody gravity;
    [SerializeField] private int second;
    void Start()
    {
        StartCoroutine(ExampleCoroutine());
        gravity = GetComponent<Rigidbody>();
        gravity.isKinematic = false;
    }
    

    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(second);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
