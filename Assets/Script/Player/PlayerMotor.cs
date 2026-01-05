using UnityEngine;
using System;

public class PlayerMotor : MonoBehaviour
{
    public static event Action<PlayerMotor> Crouched;

    private CharacterController controller;
    public bool isGrounded;
    private Vector3 playerVelocity;
    public float gravity = -9.81f;
    public float gravityMultiplier = 1f;
    public float speed = 5.0f;
    public float crouchSpeed = 4.0f;
    public float sprintmultiplier = 1.75f;
    private float baseSpeed;
    private bool lerpCrouch = false;
    private float crouchTimer = 0f;
    private bool crouching = false;
    private bool sprinting = false;
    private bool crouchActive = false;

    public float jumpHeight = 4f;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        baseSpeed = speed;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        
        ApplyGravity();

        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;

            if (crouching)
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2, p);

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && playerVelocity.y < 0f)
        {
            if (crouching && !crouchActive)
                crouchActive = true;
            else if (!crouching && crouchActive)
                crouchActive = false;
            if(sprinting)
                speed = baseSpeed * sprintmultiplier;
            else
                speed = baseSpeed;
            
            playerVelocity.y = -1f;
        }
        else
        {
            playerVelocity.y += gravity * gravityMultiplier * Time.deltaTime;
            Debug.Log($"velocity y :{playerVelocity.y}");
        }
        
        controller.Move(playerVelocity * Time.deltaTime);
        
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        if(crouchActive)
            controller.Move(transform.TransformDirection(moveDirection) * crouchSpeed * Time.deltaTime);
        else
            controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        /*
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        */

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = jumpHeight;
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        Crouched?.Invoke(this);
        lerpCrouch = true;
    }

    public void Sprint()
    {
        sprinting = !sprinting;
    }

    // --------- Nouveaux getters publics ----------
    public bool IsCrouching => crouching;
    public bool IsSprinting => sprinting;
}
