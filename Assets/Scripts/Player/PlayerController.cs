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

        [Header("References")]
        [SerializeField] private PlayerCameraController m_camera;
        [SerializeField] private PlayerAnimationController m_animController;

        [Header("AIM")]
        [SerializeField] private CrosshairController m_crosshair;

        private CharacterController m_controller;
        private PlayerInputActions m_actions;
        private bool m_isAiming;


        private Vector2 m_moveInput;
        private Vector2 m_lookInput;
        private float m_verticalVelocity;
        private bool m_isRunning;

        private void Awake()
        {
            m_controller = GetComponent<CharacterController>();

            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Move.performed += ctx => m_moveInput = ctx.ReadValue<Vector2>();
            m_actions.Player.Move.canceled += ctx => m_moveInput = Vector2.zero;

            m_actions.Player.Jump.performed += OnJump;

            m_actions.Player.Run.performed += ctx => m_isRunning = ctx.ReadValueAsButton();
            m_actions.Player.Run.canceled += ctx => m_isRunning = false;

            m_actions.Player.Look.performed += ctx => m_lookInput = ctx.ReadValue<Vector2>();
            m_actions.Player.Look.canceled += _ => m_lookInput = Vector2.zero;

            m_actions.Player.Aim.performed += _ => SetAiming(true);
            m_actions.Player.Aim.canceled += _ => SetAiming(false);

            m_actions.Player.Shoot.performed += _ => OnShoot();

        }

        private void Update()
        {

            HandleMovement();
            HandleAnimations();
            m_animController.SetGrounded(m_controller.isGrounded);
        }


        private void SetAiming(bool value)
        {
            m_isAiming = value;

            m_crosshair.SetVisible(value);
            m_camera.SetAiming(value);
            m_animController.SetAiming(value);
        }

        private void OnShoot()
        {
            if (!m_isAiming)
                return;

            m_animController.Shoot();
        }



        private void HandleMovement()
        {
            float speed = m_isRunning ? m_runSpeed : m_moveSpeed;

            Vector3 move =
                m_camera.Forward * m_moveInput.y +
                m_camera.transform.right * m_moveInput.x;

            move.y = 0f;

            m_controller.Move(move * speed * Time.deltaTime);

            if (m_controller.isGrounded)
            {
                if (m_verticalVelocity < 0f)
                    m_verticalVelocity = -2f;
            }
            else
            {
                m_verticalVelocity += m_gravity * Time.deltaTime;
            }

            m_controller.Move(Vector3.up * m_verticalVelocity * Time.deltaTime);
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (m_controller.isGrounded)
            {
                m_verticalVelocity = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
                m_animController.Jump();
            }
        }

        private void HandleAnimations()
        {
            m_animController.SetMove(m_moveInput.magnitude);
            m_animController.SetMoveDirection(m_moveInput);
            m_animController.SetRunning(m_isRunning);
        }
    }
}
