using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerNoise : MonoBehaviour
{
    private PlayerMotor motor;

    [Header("Niveaux de bruit (mètres)")]
    public float walkNoise = 4f;
    public float sprintNoise = 8f;
    public float crouchNoise = 1f;

    [HideInInspector] public float currentNoiseRadius;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        if (!motor.isGrounded)
        {
            currentNoiseRadius = 0f; // pas de bruit en l’air
            return;
        }

        if (motor.IsCrouching)
            currentNoiseRadius = crouchNoise;
        else if (motor.IsSprinting)
            currentNoiseRadius = sprintNoise;
        else
            currentNoiseRadius = walkNoise;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentNoiseRadius);
    }
}