using UnityEngine;

public class LineOfSightDetector : MonoBehaviour
{
    [SerializeField] private LayerMask m_playerLayerMask;
    [SerializeField] private float m_detectionRange = 10.0f;
    [SerializeField] private bool showDebugVisuals = true;

    /// <summary>
    /// Retourne le GameObject détecté s'il est visible dans le rayon
    /// </summary>
    public GameObject PerformDetection(GameObject potentialTarget)
    {
        if (potentialTarget == null) return null;

        Vector3 origin = transform.position;

        // Vérifie que le target est dans le rayon
        if (Vector3.Distance(origin, potentialTarget.transform.position) > m_detectionRange)
            return null;

        // Raycast vers le target pour vérifier la ligne de vue
        Vector3 direction = (potentialTarget.transform.position - origin).normalized;
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, m_detectionRange, m_playerLayerMask.value))
        {
            if (hit.collider.gameObject == potentialTarget)
            {
                if (showDebugVisuals && this.enabled)
                {
                    Debug.DrawLine(origin, potentialTarget.transform.position, Color.green, 2f);
                }
                return potentialTarget;
            }
        }

        // Debug visuel si bloqué
        if (showDebugVisuals)
            Debug.DrawLine(origin, potentialTarget.transform.position, Color.red, 2f);

        return null;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugVisuals) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionRange);
    }
}