using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Paramètres de Détection")]
    public float visionRadius = 10f;
    [Range(0, 360)] public float visionAngle = 120f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [HideInInspector] public bool playerDetected = false;
    [HideInInspector] public Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    private void Update()
    {
        playerDetected = DetectPlayer();
    }

    private bool DetectPlayer()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);

        // 🔊 Détection sonore
        PlayerNoise noise = player.GetComponent<PlayerNoise>();
        if (noise != null && dist <= noise.currentNoiseRadius)
        {
            Debug.Log($"{gameObject.name} a entendu le joueur (bruit: {noise.currentNoiseRadius}m)");
            return true;
        }

        // 👁️ Détection visuelle (cône)
        if (dist <= visionRadius)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < visionAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, dist, obstacleMask))
                {
                    Debug.Log($"{gameObject.name} a vu le joueur !");
                    return true;
                }
            }
        }

        return false;
    }

    // ----------- DEBUG GIZMOS -----------
    private void OnDrawGizmosSelected()
    {
        // Vision radius
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        // Vision angle
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRadius);

        // Debug cible
        if (playerDetected && player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
