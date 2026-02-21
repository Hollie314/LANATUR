using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public bool broken;

    [Header("Replacement")]
    public GameObject replacementPrefab;

    public void Break()
    {
        if (broken) return;

        broken = true;

        Debug.Log("Object broken");

        // Spawn replacement
        if (replacementPrefab != null)
        {
            Instantiate(
                replacementPrefab,
                transform.position,
                transform.rotation
            );
        }

        // Destroy original
        Destroy(gameObject);
    }
}