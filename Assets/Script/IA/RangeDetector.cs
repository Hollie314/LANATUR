using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class RangeDetector : MonoBehaviour
{
    [Header("Détection")]
    public float detectionRadius = 10f;
    public LayerMask detectionMask;

    [HideInInspector] public List<Transform> detectedObjects = new();

    private SphereCollider sphereCollider;

    public System.Action<Transform> OnObjectEnter;
    public System.Action<Transform> OnObjectExit;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = detectionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectionMask) != 0)
        {
            if (!detectedObjects.Contains(other.transform))
            {
                detectedObjects.Add(other.transform);
                OnObjectEnter?.Invoke(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectedObjects.Contains(other.transform))
        {
            detectedObjects.Remove(other.transform);
            OnObjectExit?.Invoke(other.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}