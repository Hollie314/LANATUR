using UnityEngine;
using UnityEngine.Events;

public class BreakableObject : MonoBehaviour
{
    public bool broken;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent unityEvent;

    [Header("Replacement")]
    public GameObject replacementPrefab;

    public void Break()
    {
        if (broken) return;
        broken = true;
        animator.SetBool("IsDestruct", true);
        unityEvent?.Invoke();   

        Debug.Log("Object broken");

        // Spawn replacement
        //if (replacementPrefab != null)
       // {
           // Instantiate(
              //  replacementPrefab,
              //  transform.position,
              //  transform.rotation
            //);
       // }

        // Destroy original
        //Destroy(gameObject);
    }
}