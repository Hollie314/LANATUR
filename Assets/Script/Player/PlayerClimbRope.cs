using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerClimbRope : MonoBehaviour
{
    private CharacterController controller;
    private PlayerMotor motor;
    private InputManager input;

    [Header("Paramètres Escalade")]
    public float climbSpeed = 3f;

    private bool nearRope = false;  
    private bool climbing = false;  
    private Transform ropeTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        motor = GetComponent<PlayerMotor>();
        input = GetComponent<InputManager>();
    }

    void Update()
    {
        // Si le joueur est proche d'une corde et appuie sur Climb
        if (nearRope && input.OnFoot.Interact.triggered)
        {
            climbing = !climbing;

            if (climbing)
            {
                motor.enabled = false; // désactive le mouvement classique
                AlignPlayerToRope();
            }
            else
            {
                motor.enabled = true; // reprend le contrôle normal
            }
        }

        if (climbing)
        {
            Climb();

            // quitter la corde si saut
            if (input.OnFoot.Jump.triggered)
            {
                climbing = false;
                motor.enabled = true;
            }
        }
    }

    void Climb()
    {
        Vector2 moveInput = input.OnFoot.Movement.ReadValue<Vector2>();
        float vertical = moveInput.y; // Z/S ou ↑/↓

        Vector3 climbDirection = new Vector3(0, vertical * climbSpeed, 0);
        controller.Move(climbDirection * Time.deltaTime);

        AlignPlayerToRope();
    }

    void AlignPlayerToRope()
    {
        if (ropeTransform != null)
        {
            Vector3 pos = transform.position;
            pos.x = ropeTransform.position.x;
            pos.z = ropeTransform.position.z;
            transform.position = pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rope"))
        {
            nearRope = true;
            ropeTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rope"))
        {
            nearRope = false;
            ropeTransform = null;

            if (climbing)
            {
                climbing = false;
                motor.enabled = true;
            }
        }
    }
}
