using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    [Header("Paramètres de Détection")]
    public float visionRadius = 10f;
    [Range(0, 360)] public float visionAngle = 120f;
    public float soundRadius = 7f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Comportement")]
    public float fleeDistance = 10f;
    public float fleeSpeed = 6f;
    public float normalSpeed = 3.5f;
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Transform player;
    private float lastAttackTime = 0f;

    private bool playerDetected = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = normalSpeed;
    }

    void Update()
    {
        playerDetected = DetectPlayer();

        if (playerDetected)
        {
            if (CompareTag("Herbivore"))
            {
                Flee();
            }
            else if (CompareTag("Carnivore"))
            {
                Attack();
            }
        }
    }

    // ----------- DÉTECTION -----------

    bool DetectPlayer()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);

        // Détection sonore
        if (dist <= soundRadius)
        {
            Debug.Log($"{gameObject.name} a entendu le joueur !");
            return true;
        }

        // Détection visuelle (cône)
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

    // ----------- FUITE -----------

    void Flee()
    {
        agent.speed = fleeSpeed;
        Vector3 dirAway = (transform.position - player.position).normalized;
        Vector3 fleePos = transform.position + dirAway * fleeDistance;
        agent.SetDestination(fleePos);
    }

    // ----------- ATTAQUE -----------

    void Attack()
    {
        agent.speed = 0; // Stop déplacement pour attaquer
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
           // Health hp = player.GetComponent<Health>();
           // if (hp != null) hp.TakeDamage(attackDamage);

            Debug.Log($"{gameObject.name} attaque le joueur et inflige {attackDamage} dégâts !");
            lastAttackTime = Time.time;
        }
        else
        {
            // Se rapproche du joueur
            agent.speed = normalSpeed;
            agent.SetDestination(player.position);
        }
    }

    // ----------- DEBUG GIZMOS -----------

    private void OnDrawGizmosSelected()
    {
        // Vision radius
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, visionRadius);

        // Sound radius
        Gizmos.color = new Color(0, 0, 1, 0.15f);
        Gizmos.DrawSphere(transform.position, soundRadius);

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
