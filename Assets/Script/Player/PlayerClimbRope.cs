using PrimeTween;
using RTGStandard;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerClimbRope : MonoBehaviour
{
    private CharacterController controller;
    private PlayerMotor motor;
    private InputManager input;

    [Header("Paramètres Escalade")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float leaveDuration = 3f;

    private bool nearRope = false;  
    private bool climbing = false;  
    private bool leavingRope = false;  
    private Transform ropeTransform;
    private Transform RopeBaseTransform;
    private Transform RopeTopTransform;
    private Transform RopeTopFinishTransform;

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
                input.canMove = false;
                leavingRope = false;
                this.gameObject.GetComponent<PlayerLook>().ClampLeftRightRotation(true);
                transform.rotation = Quaternion.Euler(0f, ropeTransform.rotation.eulerAngles.y, 0f);
                AlignPlayerToRope();
            }
            else
            {
                LeaveRope();
            }
        }

        if (climbing)
        {
            Climb();

            // quitter la corde si saut
            if (input.OnFoot.Jump.triggered)
            {
                LeaveRope();
            }
        }
    }

    void Climb()
    {

        Debug.Log("Climbing");

        Vector2 moveInput = input.OnFoot.Movement.ReadValue<Vector2>();
        float vertical = moveInput.y; // Z/S ou ↑/↓
        Debug.Log($"vertical is {vertical * climbSpeed}");

        Vector3 climbDirection = new Vector3(0, vertical * climbSpeed, 0);
        
        // Sortir en bas
        if (transform.position.y <= RopeBaseTransform.position.y && vertical < 0f)
        {
            LeaveRope();
            return;
        }
        
        // Sortir en haut
        else if (transform.position.y >= RopeTopTransform.position.y && vertical > 0f)
        {
            controller.enabled = false;
            Tween.Position(controller.transform, RopeTopFinishTransform.position, leaveDuration, Ease.InOutSine)
                .OnComplete(() => LeaveRope());
            leavingRope = true;
        }

        if (!leavingRope)
        {
            controller.Move(climbDirection * Time.deltaTime);
            AlignPlayerToRope();
        }
    }
    
    private void LeaveRope()
    {
        controller.enabled = true;
        climbing = false;
        motor.enabled = true;
        input.canMove = true;
        this.gameObject.GetComponent<PlayerLook>().ClampLeftRightRotation(false);
        return;
    }

    void AlignPlayerToRope()
    {
        if (ropeTransform != null)
        {
            Vector3 pos = transform.position;
            pos.x = ropeTransform.position.x;
            pos.z = ropeTransform.position.z;
            transform.position = pos;
            controller.transform.position = pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rope"))
        {
            nearRope = true;
            ropeTransform = other.transform;
            RopeBaseTransform = other.transform.GetChild(0).transform;
            RopeTopTransform = other.transform.GetChild(1).transform;
            RopeTopFinishTransform = other.transform.GetChild(2).transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rope"))
        {
            nearRope = false;
            ropeTransform = null;

            Debug.Log("TriggerExit");
            if (climbing)
            {
                climbing = false;
                motor.enabled = true;
                input.canMove = true;
                this.gameObject.GetComponent<PlayerLook>().ClampLeftRightRotation(false);
            }
        }
    }
}
