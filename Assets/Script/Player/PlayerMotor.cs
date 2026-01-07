using UnityEngine;
using System;

public class PlayerMotor : MonoBehaviour
{
    public static event Action<PlayerMotor> Crouched;

    private CharacterController controller;
    public bool isGrounded;
    private Vector3 playerVelocity;

    [SerializeField] private float groundedRayLength;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float MaxFallGravity = -20f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float crouchSpeed = 4.0f;
    [SerializeField] private float sprintmultiplier = 1.75f;
    
    private float baseSpeed;
    private bool lerpCrouch = false;
    private float crouchTimer = 0f;
    private bool crouching = false;
    private bool sprinting = false;
    private bool crouchActive = false;
    private float gravityMultiplierUsed;
    private bool isJumping = false;
    private bool canJump = true;

    [SerializeField] private float minJumpHeight = 4f;
    [SerializeField] private float maxJumpHeight = 6f;
    [SerializeField] private float CrouchminJumpHeight = 3f;
    [SerializeField] private float CrouchmaxJumpHeight = 4.5f;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        baseSpeed = speed;
    }

    void Update()
    {
        canJump = CheckIsGrounded(groundedRayLength);
        isGrounded = controller.isGrounded;
        ApplyGravity();
        if(isJumping)
            PushJump();
        

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

    private bool CheckIsGrounded(float rayLength)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            return true;
        }

        return false;
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
            gravityMultiplierUsed = MaxFallGravity;
        }
        else
        {
            if (playerVelocity.y > 1f)
                playerVelocity.y += gravity * Time.deltaTime;
            else
            {
                float easedGravity = Mathf.Pow(gravityMultiplierUsed, 2);
                gravityMultiplierUsed = Mathf.Lerp(gravity, MaxFallGravity, easedGravity);
                playerVelocity.y += MaxFallGravity * Time.deltaTime;
            }
            Debug.Log($"velocity y :{playerVelocity.y}");
        }
        
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

        controller.Move(playerVelocity * Time.deltaTime);
    }
    
    public void JumpStart()
    {
        Debug.Log("Jump");
        if(!canJump)
            return;
        if(crouchActive)
        {
            playerVelocity.y = CrouchminJumpHeight;
            isJumping = true;
        }
        if (canJump)
        {
            playerVelocity.y = minJumpHeight;
            isJumping = true;
        }
    }

    private void PushJump()
    {
        if (crouchActive)
        {
            playerVelocity.y -= gravity * Time.deltaTime;
            playerVelocity.y += CrouchminJumpHeight * Time.deltaTime;
            if (playerVelocity.y > CrouchmaxJumpHeight)
            {
                playerVelocity.y = CrouchmaxJumpHeight;
                isJumping = false;
            }

            return;
        }
        
        playerVelocity.y -= gravity * Time.deltaTime;
        playerVelocity.y += minJumpHeight * Time.deltaTime;
        if (playerVelocity.y > maxJumpHeight)
        {
            playerVelocity.y = maxJumpHeight;
            isJumping = false;
        }

        return;
    }
    
    public void JumpCanceled()
    {
        Debug.Log("Jump canceled");
        isJumping =  false;
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
