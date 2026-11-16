using System;
using UnityEngine;
using Hitrush.Core;

namespace Hitrush.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Lane Settings")]
        [SerializeField] private float[] lanePositions = { -2.5f, 0f, 2.5f };
        [SerializeField] private float laneTransitionSpeed = 10f;
        
        [Header("Movement Settings")]
        [SerializeField] private float baseSpeed = 10f;
        [SerializeField] private float maxSpeed = 25f;
        [SerializeField] private float speedIncreaseRate = 0.5f; // per 1000 distance
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float gravity = -25f;
        [SerializeField] private float jumpDuration = 0.6f;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Slide Settings")]
        [SerializeField] private float slideDuration = 0.8f;
        [SerializeField] private float slideColliderHeight = 0.5f;
        [SerializeField] private float normalColliderHeight = 2f;
        [SerializeField] private float slideColliderCenterY = 0.25f;
        [SerializeField] private float normalColliderCenterY = 1f;
        
        [Header("Components")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;
        
        // State
        private int currentLaneIndex = 1; // Middle lane
        private float targetX;
        private float currentSpeed;
        private float verticalVelocity;
        private bool isGrounded;
        private bool isJumping;
        private bool isSliding;
        private float slideTimer;
        private float jumpTimer;
        private float distanceTraveled;
        
        // Input
        private PlayerInputHandler inputHandler;
        private PlayerAnimationController animationController;
        private PlayerCollisionHandler collisionHandler;
        
        // Events
        public Action<float> OnSpeedChanged;
        public Action<int> OnLaneChanged;
        public Action OnJump;
        public Action OnSlide;
        public Action OnDeath;
        
        private void Awake()
        {
            inputHandler = GetComponent<PlayerInputHandler>();
            animationController = GetComponent<PlayerAnimationController>();
            collisionHandler = GetComponent<PlayerCollisionHandler>();
            
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
                
            targetX = lanePositions[currentLaneIndex];
            currentSpeed = baseSpeed;
        }
        
        private void Start()
        {
            if (inputHandler != null)
            {
                inputHandler.OnMoveLeft += MoveLeft;
                inputHandler.OnMoveRight += MoveRight;
                inputHandler.OnJump += Jump;
                inputHandler.OnSlide += Slide;
            }
        }
        
        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            
            HandleLaneMovement();
            HandleJumpAndGravity();
            HandleSlide();
            HandleSpeedIncrease();
            HandleForwardMovement();
        }
        
        private void HandleLaneMovement()
        {
            float targetLaneX = lanePositions[currentLaneIndex];
            targetX = Mathf.Lerp(targetX, targetLaneX, laneTransitionSpeed * Time.deltaTime);
            
            Vector3 pos = transform.position;
            pos.x = targetX;
            transform.position = pos;
        }
        
        private void HandleForwardMovement()
        {
            Vector3 move = Vector3.forward * currentSpeed * Time.deltaTime;
            move.y = verticalVelocity * Time.deltaTime;
            characterController.Move(move);
            
            distanceTraveled += currentSpeed * Time.deltaTime;
        }
        
        private void HandleSpeedIncrease()
        {
            float targetSpeed = baseSpeed + (distanceTraveled / 1000f) * speedIncreaseRate;
            currentSpeed = Mathf.Min(targetSpeed, maxSpeed);
            OnSpeedChanged?.Invoke(currentSpeed);
        }
        
        private void HandleJumpAndGravity()
        {
            // Ground check
            isGrounded = characterController.isGrounded;
            
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Small downward force to keep grounded
                
                if (isJumping)
                {
                    isJumping = false;
                    animationController?.SetJumping(false);
                }
            }
            
            if (!isGrounded)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
            
            jumpTimer -= Time.deltaTime;
        }
        
        private void HandleSlide()
        {
            if (isSliding)
            {
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0f)
                {
                    EndSlide();
                }
            }
        }
        
        public void MoveLeft()
        {
            if (currentLaneIndex > 0)
            {
                currentLaneIndex--;
                OnLaneChanged?.Invoke(currentLaneIndex);
            }
        }
        
        public void MoveRight()
        {
            if (currentLaneIndex < lanePositions.Length - 1)
            {
                currentLaneIndex++;
                OnLaneChanged?.Invoke(currentLaneIndex);
            }
        }
        
        public void Jump()
        {
            if (isGrounded && !isSliding && !isJumping)
            {
                isJumping = true;
                verticalVelocity = jumpForce;
                jumpTimer = jumpDuration;
                animationController?.SetJumping(true);
                OnJump?.Invoke();
            }
        }
        
        public void Slide()
        {
            if (isGrounded && !isSliding && !isJumping)
            {
                isSliding = true;
                slideTimer = slideDuration;
                
                // Reduce collider height
                characterController.height = slideColliderHeight;
                characterController.center = new Vector3(0, slideColliderCenterY, 0);
                
                animationController?.SetSliding(true);
                OnSlide?.Invoke();
            }
        }
        
        private void EndSlide()
        {
            isSliding = false;
            
            // Restore collider
            characterController.height = normalColliderHeight;
            characterController.center = new Vector3(0, normalColliderCenterY, 0);
            
            animationController?.SetSliding(false);
        }
        
        public void Die()
        {
            if (GameManager.Instance.CurrentState == GameState.GameOver) return;
            
            GameManager.Instance.GameOver();
            animationController?.SetDead(true);
            enabled = false;
            OnDeath?.Invoke();
        }
        
        public float GetDistanceTraveled() => distanceTraveled;
        public float GetCurrentSpeed() => currentSpeed;
        public int GetCurrentLane() => currentLaneIndex;
        public bool IsGrounded() => isGrounded;
        public bool IsSliding() => isSliding;
        public bool IsJumping() => isJumping;
        
        private void OnDestroy()
        {
            if (inputHandler != null)
            {
                inputHandler.OnMoveLeft -= MoveLeft;
                inputHandler.OnMoveRight -= MoveRight;
                inputHandler.OnJump -= Jump;
                inputHandler.OnSlide -= Slide;
            }
        }
    }
}
