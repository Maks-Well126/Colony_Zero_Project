using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float m_moveSpeed = 5f;
        [SerializeField] private float m_runSpeed = 9f;
        [SerializeField] private float m_jumpHeight = 1.5f;
        [SerializeField] private float m_gravity = -9.81f;

        [Header("Camera")]
        [SerializeField] private Transform m_cameraRoot;
        [SerializeField] private float m_lookSensitivity = 0.1f;

        private CharacterController m_controller;
        private PlayerInputActions m_actions;

        private Vector2 m_moveInput;
        private Vector2 m_lookInput;

        private float m_verticalVelocity;
        private float m_cameraPitch;

        private bool m_isRunning;

        private void Awake()
        {
            m_controller = GetComponent<CharacterController>();

            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Move.performed += OnMove;
            m_actions.Player.Move.canceled += OnMove;

            m_actions.Player.Look.performed += OnLook;
            m_actions.Player.Look.canceled += OnLook;

            m_actions.Player.Jump.performed += OnJump;

            m_actions.Player.Run.performed += OnRun;
            m_actions.Player.Run.canceled += OnRun;
        }

        private void Update()
        {
            HandleMovement();
            HandleCamera();
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            m_moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            m_lookInput = ctx.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (m_controller.isGrounded)
            {
                m_verticalVelocity = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
            }
        }

        private void OnRun(InputAction.CallbackContext ctx)
        {
            m_isRunning = ctx.ReadValueAsButton();
        }

        private void HandleMovement()
        {
            if (m_controller.isGrounded && m_verticalVelocity < 0f)
            {
                m_verticalVelocity = -2f;
            }

            float speed = m_isRunning ? m_runSpeed : m_moveSpeed;

            Vector3 move = transform.right * m_moveInput.x + transform.forward * m_moveInput.y;
            m_controller.Move(move * speed * Time.deltaTime);

            m_verticalVelocity += m_gravity * Time.deltaTime;
            m_controller.Move(Vector3.up * m_verticalVelocity * Time.deltaTime);
        }

        private void HandleCamera()
        {
            float mouseX = m_lookInput.x * m_lookSensitivity;
            float mouseY = m_lookInput.y * m_lookSensitivity;

            m_cameraPitch -= mouseY;
            m_cameraPitch = Mathf.Clamp(m_cameraPitch, -80f, 80f);

            m_cameraRoot.localRotation = Quaternion.Euler(m_cameraPitch, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}
