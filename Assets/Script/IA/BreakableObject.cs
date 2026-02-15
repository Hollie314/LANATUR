using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public bool broken;

    public void Break()
    {
        if (broken) return;

        broken = true;

        Debug.Log("Object broken");

        Destroy(gameObject);
    }
}