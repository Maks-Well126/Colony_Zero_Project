using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private PlayerConfig m_config;

        [Header("References")]
        [SerializeField] private PlayerCameraController m_camera;
        [SerializeField] private PlayerAnimationController m_animController;
        [SerializeField] private CrosshairController m_crosshair;
        [SerializeField] private Rig m_aimRig;
        [SerializeField] private Transform m_aimTarget;
        [SerializeField] private HealthComponent m_health;
        [SerializeField] private DamageVignetteController m_vignette;

        private CharacterController m_controller;
        private PlayerInputActions m_actions;

        private PlayerState m_currentState = PlayerState.Idle;
        public PlayerState CurrentState => m_currentState;

        private Vector2 m_moveInput;
        private float m_verticalVelocity;
        private bool m_isRunning;
        private float m_currentRigWeight;
        private bool m_isDead;

        #region Initialization

        private void Awake()
        {
            m_controller = GetComponent<CharacterController>();

            // Инициализация здоровья из конфига
            m_health.Initialize(m_config.MaxHealth);

            m_health.HealthChanged += OnHealthChanged;
            m_health.Damaged += OnDamaged;
            m_health.Died += OnDeath;

            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Move.performed += ctx => m_moveInput = ctx.ReadValue<Vector2>();
            m_actions.Player.Move.canceled += _ => m_moveInput = Vector2.zero;

            m_actions.Player.Jump.performed += OnJump;

            m_actions.Player.Run.performed += ctx => m_isRunning = ctx.ReadValueAsButton();
            m_actions.Player.Run.canceled += _ => m_isRunning = false;

            m_actions.Player.Aim.performed += _ =>
            {
                if (m_currentState == PlayerState.Reloading || m_isDead)
                    return;

                SetState(PlayerState.Aiming);
            };

            m_actions.Player.Aim.canceled += _ =>
            {
                if (m_currentState == PlayerState.Aiming)
                    SetState(PlayerState.Idle);
            };
        }

        #endregion

        private void Update()
        {
            if (m_isDead)
                return;

            HandleMovement();
            HandleAnimations();
            UpdateRig();
            UpdateAimTarget();
            CheckCrosshairTarget();

            m_animController.SetGrounded(m_controller.isGrounded);
        }

        #region State Machine

        public void SetState(PlayerState newState)
        {
            if (m_currentState == newState || m_isDead)
                return;

            m_currentState = newState;

            switch (m_currentState)
            {
                case PlayerState.Idle:
                    ApplyAiming(false);
                    break;

                case PlayerState.Aiming:
                    ApplyAiming(true);
                    break;

                case PlayerState.Reloading:
                    ApplyAiming(false);
                    m_animController.TriggerReload();
                    break;

                case PlayerState.Shooting:
                    break;
            }
        }

        private void ApplyAiming(bool value)
        {
            m_crosshair.SetVisible(value);
            m_camera.SetAiming(value);
            m_animController.SetAiming(value);
        }

        #endregion

        #region Movement

        private void HandleMovement()
        {
            float speed = m_isRunning ? m_config.RunSpeed : m_config.MoveSpeed;

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
                m_verticalVelocity += m_config.Gravity * Time.deltaTime;
            }

            m_controller.Move(Vector3.up * m_verticalVelocity * Time.deltaTime);
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (!m_controller.isGrounded || m_isDead)
                return;

            m_verticalVelocity = Mathf.Sqrt(
                m_config.JumpHeight * -2f * m_config.Gravity
            );

            m_animController.Jump();
        }

        #endregion

        #region Aim & Rig

        private void UpdateRig()
        {
            float target = m_currentState == PlayerState.Aiming ? 1f : 0f;

            m_currentRigWeight = Mathf.Lerp(
                m_currentRigWeight,
                target,
                Time.deltaTime * m_config.RigSmoothSpeed
            );

            m_aimRig.weight = m_currentRigWeight;
        }

        private void UpdateAimTarget()
        {
            Vector3 targetPos =
                m_camera.transform.position +
                m_camera.Forward * m_config.AimDistance;

            m_aimTarget.position = targetPos;
        }

        private void CheckCrosshairTarget()
        {
            if (m_currentState != PlayerState.Aiming)
            {
                m_crosshair.SetEnemyTarget(false);
                return;
            }

            if (Physics.Raycast(
                m_camera.transform.position,
                m_camera.Forward,
                out RaycastHit hit,
                100f))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    m_crosshair.SetEnemyTarget(true);
                    return;
                }
            }

            m_crosshair.SetEnemyTarget(false);
        }

        #endregion

        #region Animation

        private void HandleAnimations()
        {
            m_animController.SetMove(m_moveInput.magnitude);
            m_animController.SetMoveDirection(m_moveInput);
            m_animController.SetRunning(m_isRunning);
        }

        public void TriggerShootAnimation()
        {
            if (!m_isDead)
                m_animController.Shoot();
        }

        #endregion

        #region Health Events

        private void OnHealthChanged(float current, float max)
        {
            m_vignette.OnHealthChanged(current, max);
        }

        private void OnDamaged()
        {
            if (m_isDead)
                return;

            m_animController.TriggerHit();
            m_vignette.FlashDamage();
        }

        private void OnDeath()
        {
            m_isDead = true;

            SetState(PlayerState.Idle);

            m_animController.TriggerDeath();
            m_vignette.OnDeath();

            m_actions.Player.Disable();
        }

        #endregion
    }
}