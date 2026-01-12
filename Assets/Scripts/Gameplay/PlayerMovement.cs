using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plat.Gameplay
{
    public class PlayerMovement : MonoBehaviour, IPlayerComponent
    {
        
        public float T { get; private set; }

        public float TProperty => tfield;
        
        private float tfield;
        
        public PlayerController Controller { get; set; }
        
        public Vector2 Direction { get; private set; }
        public Vector2 TargetVelocity { get; private set; }
        public Vector2 CurrentVelocity { get; private set; }
        public Vector2 AdditionalVelocity { get; private set; }
        
        public bool IsGrounded { get; private set; }
        public Vector2 GroundNormal { get; private set; }
        
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb2d;
        
        [Header("Movement")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float directionAlignDamping;
        
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float jumpForce;
        [SerializeField] private int coyoteTime;
        
        [Header("Gravity")]
        [SerializeField] private float gravityMultiplier;

        [Header("Raycasts")]
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private LayerMask groundLayer;
        
        private void Update()
        {
            ComputeDirection();
            ComputeTargetVelocity();
            ComputeGravity();
        }

        private void FixedUpdate()
        {
            var list = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                list.Add(34);
            }
            CheckGround();
            ApplyVelocity();
        }

        private void ComputeDirection()
        {
            Vector2 direction = Controller.PlayerControls.MoveInput;
            direction.y = 0;
            
            Direction = direction.normalized;
        }

        private void ComputeTargetVelocity()
        {
            Vector2 lastTargetVelocity = TargetVelocity;
            
            bool wantsToStop = Direction.sqrMagnitude < 0.1f;
            
            float finalTargetSpeed = wantsToStop ? 0 : maxSpeed;
            
            Vector2 finalTargetVelocity = Direction * finalTargetSpeed;
            
            Vector2 targetDirection = Vector2.Lerp(
                lastTargetVelocity, 
                finalTargetVelocity, 
                directionAlignDamping * Time.deltaTime).normalized;
            
            float lastTargetSpeed = TargetVelocity.magnitude;
            float delta = wantsToStop ? -deceleration : acceleration;
            
            //Ajout de l'acceleration en fonction du temps en seconde
            float targetSpeed = lastTargetSpeed + delta * Time.deltaTime;
            targetSpeed = Mathf.Clamp(targetSpeed, 0, maxSpeed);
            
            TargetVelocity = targetDirection * targetSpeed;
        }

        private void ComputeGravity()
        {
            Vector2 worldGravity = Physics2D.gravity;

            Vector2 currentGravity = worldGravity * gravityMultiplier;
            CurrentVelocity += currentGravity;

            if (Controller.PlayerControls.WantsToJump && IsGrounded)
            {
                Vector2 temp = CurrentVelocity;
                temp.y = jumpForce;
                
                CurrentVelocity = temp;
            }
        }
        
        private void ApplyVelocity()
        {
            Vector2 finalTargetVelocity = Vector3.ProjectOnPlane(TargetVelocity, GroundNormal);
            Vector2 verticalVelocity = Vector3.Project(CurrentVelocity, GroundNormal);

            float dot = Vector2.Dot(verticalVelocity.normalized, Physics2D.gravity.normalized);
            
            if(IsGrounded && dot >= 0)
                verticalVelocity = Vector2.zero;
            
            CurrentVelocity = finalTargetVelocity + verticalVelocity;
            
            rb2d.linearVelocity = CurrentVelocity;
        }

        private void CheckGround()
        {
            RaycastHit2D hit2D = Physics2D.Raycast(rb2d.position, Physics2D.gravity, groundCheckDistance, groundLayer);
            if (hit2D)
            {
                IsGrounded = true;
                GroundNormal = hit2D.normal;
            }
            else
            {
                IsGrounded = false;
                //Inutile mais peut etre utile
                GroundNormal = -Physics2D.gravity.normalized;
            }
        }


        private void OnDrawGizmos()
        {
            Vector2 rayDirection = Physics2D.gravity.normalized * groundCheckDistance;
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(rb2d.position, rayDirection);
        }
    }
}