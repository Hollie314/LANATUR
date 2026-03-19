using UnityEngine;

public class LineOfSightDetector : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_playerLayerMask;
    [SerializeField]
    private float m_detectionRange = 10.0f;
    [SerializeField]
    private float m_detectionHeight = 3f;

    [SerializeField] private bool showDebugVisuals = true;

    // Optionnel : assigner un target dans l'Inspector pour visualiser
    [SerializeField] private GameObject debugTarget;

    /// <summary>
    /// Retourne le GameObject détecté s'il est visible
    /// </summary>
    public GameObject PerformDetection(GameObject potentialTarget)
    {
        if (potentialTarget == null) return null;

        Collider targetCollider = potentialTarget.GetComponent<Collider>();
        if (targetCollider == null) return null;

        Vector3 origin = transform.position + Vector3.up * m_detectionHeight;

        // Points à viser sur le target
        Vector3[] pointsToCheck = new Vector3[]
        {
            potentialTarget.transform.position,        // centre
            targetCollider.bounds.max,                 // haut
            targetCollider.bounds.min,                 // bas
        };

        foreach (var point in pointsToCheck)
        {
            Vector3 direction = (point - origin).normalized;
            float distance = Vector3.Distance(origin, point);

            if (distance > m_detectionRange) continue;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, m_playerLayerMask.value))
            {
                if (hit.collider.gameObject == potentialTarget)
                {
                    if (showDebugVisuals && this.enabled)
                    {
                        Debug.DrawLine(origin, point, Color.green, 2f);
                    }
                    return potentialTarget;
                }
            }
            else
            {
                if (showDebugVisuals)
                    Debug.DrawRay(origin, direction * distance, Color.red, 2f);
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugVisuals) return;

        Vector3 origin = transform.position + Vector3.up * m_detectionHeight;

        // Affiche la zone de détection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, m_detectionRange);

        // Optionnel : afficher la ligne vers un target assigné pour debug
        if (debugTarget != null)
        {
            Vector3 direction = (debugTarget.transform.position - origin).normalized;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(origin, direction * m_detectionRange);
        }
    }
}