using UnityEngine;
using UnityEngine.AI;

public class AnimalBehavior : MonoBehaviour
{
    public enum BehaviorType { Passive, Herbivore, Carnivore }
    [Header("Type de comportement")]
    public BehaviorType behaviorType = BehaviorType.Passive;

    [Header("Comportement général")]
    public float fleeDistance = 10f;
    public float fleeSpeed = 6f;
    public float normalSpeed = 3.5f;
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private PlayerDetection detection;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        detection = GetComponent<PlayerDetection>();
        agent.speed = normalSpeed;
    }

    void Update()
    {
        if (detection == null || detection.player == null)
            return;

        if (detection.playerDetected)
        {
            switch (behaviorType)
            {
                case BehaviorType.Herbivore:
                    Flee();
                    break;
                case BehaviorType.Carnivore:
                    Attack();
                    break;
                case BehaviorType.Passive:
                default:
                    // Ne rien faire
                    break;
            }
        }
    }

    // ----------- FUITE -----------
    void Flee()
    {
        agent.speed = fleeSpeed;
        Vector3 dirAway = (transform.position - detection.player.position).normalized;
        Vector3 fleePos = transform.position + dirAway * fleeDistance;
        agent.SetDestination(fleePos);
    }

    // ----------- ATTAQUE -----------
    void Attack()
    {
        float dist = Vector3.Distance(transform.position, detection.player.position);

        if (dist <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            PlayerHealth hp = detection.player.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(attackDamage);

            Debug.Log($"{gameObject.name} attaque le joueur et inflige {attackDamage} dégâts !");
            lastAttackTime = Time.time;
        }
        else
        {
            agent.speed = normalSpeed;
            agent.SetDestination(detection.player.position);
        }
    }
}
