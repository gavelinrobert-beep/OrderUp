using UnityEngine;
using Mirror;

namespace OrderUp.Player
{
    /// <summary>
    /// PlayerController handles player movement and controls
    /// Uses CharacterController for movement and Mirror for networking
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float gravity = -9.81f;

        [Header("References")]
        private CharacterController characterController;
        private Vector3 velocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Only allow local player to control their character
            if (!isLocalPlayer) return;

            HandleMovement();
            HandleRotation();
            ApplyGravity();
        }

        private void HandleMovement()
        {
            // Get input from WASD or arrow keys
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculate movement direction relative to camera
            Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                // Move the character
                Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
                characterController.Move(move);
            }
        }

        private void HandleRotation()
        {
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                // Calculate target rotation based on movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                
                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        private void ApplyGravity()
        {
            // Apply gravity
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small downward force to keep grounded
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        // Called when this player spawns on a client
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            // Set camera to follow this player
            if (Camera.main != null)
            {
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTarget(transform);
                }
            }

            Debug.Log($"PlayerController: Local player spawned at {transform.position}");
        }
    }
}
