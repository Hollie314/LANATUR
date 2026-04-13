using UnityEngine;

public class BreakObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnCollisionEnter(Collision collision)
    {
        TryBreakObject(collision.collider);
    }
    void TryBreakObject(Collider col)
    {
        //Debug.Log("Lamantin broke object in state: ");

        BreakableObject breakable = col.GetComponent<BreakableObject>();

        if (breakable != null)
        {
            breakable.Break();
        }
    }
   
}
