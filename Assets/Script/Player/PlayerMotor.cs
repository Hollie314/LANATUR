using UnityEngine;
using System;

public class PlayerMotor : MonoBehaviour
{
    public static event Action<PlayerMotor> Crouched;

    public bool isGrounded { get; private set; }
    private CharacterController controller;
    private Vector3 playerVelocity;

    [Header("Noise")]
    // NoiseRadius[SerializeField] private float noiseRadius_Running;
    [SerializeField] private float noiseInsity_Walking;
    [SerializeField] private float noiseInsity_Crouching;
    [SerializeField] private float noiseInsity_Running;
    [SerializeField] private float noiseRadius_Walking;
    [SerializeField] private float noiseRadius_Crouching;
    [SerializeField] private float noiseRadius_Running;
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
    
    [Header("SFX")]
    // Movements
    [SerializeField] private AudioSource playerMoveAudioSource;
    [SerializeField] private AudioSource playerJumpAudioSource;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip fallSFX;
    [SerializeField] private AudioClip CrouchSFX;
    [SerializeField] private AudioClip WalkSFX;
    [SerializeField] private AudioClip SprintSFX;
    
    // private var
    private bool lerpCrouch = false;
    private float crouchTimer = 0f;
    private bool crouching = false;
    private bool sprinting = false;
    private bool crouchActive = false;
    private bool isJumping = false;
    private bool canJump = true;
    
    private Vector3 groundNormal;
    private Vector3 playerInput;


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckIsGrounded(groundedRayLength, out RaycastHit hit);
        if (isGrounded)
            groundNormal = hit.normal;
        else
            groundNormal = Vector3.up;
        
        canJump = isGrounded;
        ApplyGravity();
        
        CollisionFlags collisions = controller.Move(playerVelocity * Time.deltaTime);
    }

    void Update()
    {
        HandleNoise();
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

    private bool CheckIsGrounded(float rayLength, out RaycastHit groundHit)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, rayLength))
        {
            //SFX
            if(!canJump)
            {
                playerJumpAudioSource.clip = fallSFX;
                playerJumpAudioSource.Play();
            }
            return true;
        }

        return false;
    }

    private void ApplyGravity()
    {
        Vector3 groundVelocity = Vector3.ProjectOnPlane(playerVelocity, groundNormal);
        Vector3 verticalVelocity = playerVelocity - groundVelocity;
        
        if (isGrounded && verticalVelocity.y <= 0f)
        {
            if (crouching && !crouchActive)
                crouchActive = true;
            else if (!crouching && crouchActive)
                crouchActive = false;

            verticalVelocity = Vector3.zero;
            playerVelocity = groundVelocity + verticalVelocity;
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
            if (playerVelocity.y <= MaxFallGravity)
                playerVelocity.y = MaxFallGravity;
            
            /*
            if (playerVelocity.y > 0.2f)
            else
            {
                float easedGravity = Mathf.Pow(gravityMultiplierUsed, 2);
                gravityMultiplierUsed = Mathf.Lerp(gravity, MaxFallGravity, easedGravity);
                playerVelocity.y += MaxFallGravity * Time.deltaTime;
            }
            */
        }
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = new()
        {
            x = input.x,
            z = input.y
        };

        Vector3 groundVelocity = Vector3.ProjectOnPlane(playerVelocity, groundNormal);
        Vector3 verticalVelocity = playerVelocity - groundVelocity;
        float targetSpeed = crouchActive ? crouchSpeed : speed;
        if (sprinting)
            targetSpeed *= sprintmultiplier;

        Vector3 groundInput = Vector3.ProjectOnPlane(transform.TransformDirection(moveDirection), groundNormal).normalized;
        groundVelocity = groundInput * targetSpeed;
        
        playerInput = groundInput;
        playerVelocity = groundVelocity + verticalVelocity;
        
    }

    private void HandleNoise()
    {
        if (playerInput.sqrMagnitude >= 0.01f)
        {
            if (crouchActive)
            {
                Vector3 p1 = transform.position + controller.center;
                _makeNoise.Noise(
                    p1,
                    noiseRadius_Crouching,noiseInsity_Crouching,
                    this.gameObject,
                    playerMoveAudioSource,
                    CrouchSFX
                );
                Debug.Log("NOISE crouch");
                
                //SFX
                if (!playerMoveAudioSource.isPlaying || !playerMoveAudioSource.clip == CrouchSFX)
                {
                    playerMoveAudioSource.clip = CrouchSFX;
                    playerMoveAudioSource.Play();
                }
            }
            else
            {
                if (sprinting)
                {
                    Vector3 p1 = transform.position + controller.center;
                    _makeNoise.Noise(
                        p1,
                        noiseRadius_Running,
                        noiseInsity_Running,
                        this.gameObject,
                        playerMoveAudioSource,
                        SprintSFX
                    );
                    Debug.Log("NOISE sprint");
                    
                    //SFX
                    if (!playerMoveAudioSource.isPlaying || !playerMoveAudioSource.clip == SprintSFX)
                    {
                        playerMoveAudioSource.clip = SprintSFX;
                        playerMoveAudioSource.Play();
                    }
                }
                else
                {
                    Vector3 p1 = transform.position + controller.center;
                    _makeNoise.Noise(
                        p1,
                        noiseRadius_Walking,
                        noiseInsity_Walking,
                        this.gameObject,
                        playerMoveAudioSource,
                        WalkSFX
                    );
                    Debug.Log("NOISE walk");
                    
                    //SFX
                    if (!playerMoveAudioSource.isPlaying || !playerMoveAudioSource.clip == WalkSFX)
                    {
                        playerMoveAudioSource.clip = WalkSFX;
                        playerMoveAudioSource.Play();
                    }
                }
            }
        }
        //SFX
        else
            playerMoveAudioSource.Stop();
    }

    public void JumpStart()
    {
        Debug.Log("Jump");
        if (!canJump)
            return;

        //SFX
        playerJumpAudioSource.clip = jumpSFX;
        playerJumpAudioSource.Play();

        playerVelocity.y = crouchActive ? CrouchJumpHeight : JumpHeight;
        isJumping = true;
    }

    public void JumpCanceled()
    {
        Debug.Log("Jump canceled");
        isJumping =  false;
        playerVelocity.y *= 0.5f;
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
