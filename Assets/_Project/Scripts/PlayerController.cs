using UnityEngine;

namespace BasketballAudition
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float gravity = -9.81f;
        
        [Header("Look")]
        public float mouseSensitivity = 2f;
        public float maxLookAngle = 89f;
        public Transform playerCamera;

        private CharacterController characterController;
        private float pitch = 0f;
        private Vector3 velocity;
        
        public Transform targetHoop;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);
            
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
            
            if (playerCamera != null)
            {
                playerCamera.localEulerAngles = new Vector3(pitch, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            characterController.Move(move * moveSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        public void RandomizePosition()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            
            characterController.enabled = false;
            
            float randX = Random.Range(-4.5f, 4.5f);
            float randZ = Random.Range(3f, 8f);
            
            transform.position = new Vector3(randX, 1.5f, randZ);
            
            if (targetHoop != null)
            {
                Vector3 targetPath = targetHoop.position - transform.position;
                targetPath.y = 0;
                if (targetPath != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(targetPath);
                    targetHoop.rotation = Quaternion.LookRotation(targetPath, Vector3.up);
                }
            }
            
            characterController.enabled = true;
        }
    }
}
