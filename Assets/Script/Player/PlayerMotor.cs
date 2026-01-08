using UnityEngine;
using System;

public class PlayerMotor : MonoBehaviour
{
    public static event Action<PlayerMotor> Crouched;

    public bool isGrounded { get; private set; }
    private CharacterController controller;
    private Vector3 playerVelocity;

    [Header("NoiseRadius")]
    // NoiseRadius
    [SerializeField] private float noiseRadius_Running;
    [SerializeField] private float noiseRadius_Walking;
    [SerializeField] private float noiseRadius_Crouching;
    [SerializeField] MakeNoise _makeNoise;
    
    [Header("Movements")]
    // Movements
    [SerializeField] private float groundedRayLength;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float MaxFallGravity = -20f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float crouchSpeed = 4.0f;
    [SerializeField] private float sprintmultiplier = 1.75f;
    [SerializeField, Range(0f, 0.75f)] private float coyoteTime = 1.75f;
    
    [Header("Jump")]
    // Movements
    [SerializeField] private float JumpHeight = 6f;
    [SerializeField] private float CrouchJumpHeight = 4.5f;
    
    // private var
    private float baseSpeed;
    private bool lerpCrouch = false;
    private float crouchTimer = 0f;
    private bool crouching = false;
    private bool sprinting = false;
    private bool crouchActive = false;
    private float gravityMultiplierUsed;
    private bool isJumping = false;
    private bool canJump = true;
    


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
        }
        
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        if (moveDirection != Vector3.zero)
        {
            if (crouchActive)
            {
                controller.Move(transform.TransformDirection(moveDirection) * crouchSpeed * Time.deltaTime);
                Vector3 p1 = transform.position + controller.center;
                _makeNoise.Noise(this.gameObject, p1, noiseRadius_Crouching);
                Debug.Log("NOISE crouch");
            }
            else
            {
                controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
                if (sprinting)
                {
                    Vector3 p1 = transform.position + controller.center;
                    _makeNoise.Noise(this.gameObject, p1, noiseRadius_Walking);
                    Debug.Log("NOISE sprint");
                }
                else
                {
                    Vector3 p1 = transform.position + controller.center;
                    _makeNoise.Noise(this.gameObject, p1, noiseRadius_Running);
                    Debug.Log("NOISE walk");
                    
                }
            }
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }
    
    public void JumpStart()
    {
        Debug.Log("Jump");
        if(!canJump)
            return;
        if(crouchActive)
        {
            playerVelocity.y = CrouchJumpHeight;
            isJumping = true;
        }
        if (canJump)
        {
            playerVelocity.y = JumpHeight;
            isJumping = true;
        }
    }
    
    public void JumpCanceled()
    {
        Debug.Log("Jump canceled");
        isJumping =  false;
        playerVelocity *= 0.5f;
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
