using System.Collections.Generic;
using UnityEngine;

public class RangeDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] public List<LayerMask> detectionMask;
    [SerializeField] private bool showDebugVisuals = true;

    public GameObject DetectedTarget
    {
        get;
        set;
    }

    public GameObject UpdateDetector(int layer)
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            detectionMask[layer]
        );

        foreach (Collider collider in colliders)
        {
            // Ignore soi-même et ses enfants
            if (collider.transform == transform ||
                collider.transform.IsChildOf(transform))
            {
                continue;
            }

            DetectedTarget = collider.gameObject;
            return DetectedTarget;
        }

        DetectedTarget = null;
        return null;
    }
    // Debug visualization
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals || this.enabled == false) return;

        Gizmos.color = DetectedTarget ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }
}
