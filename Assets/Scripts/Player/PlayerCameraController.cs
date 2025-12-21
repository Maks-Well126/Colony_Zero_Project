using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_player;
        [SerializeField] private Transform m_cameraRoot;
        [SerializeField] private Camera m_camera;

        [Header("Look")]
        [SerializeField] private float m_sensitivity = 2f;
        [SerializeField] private float m_minPitch = -30f;
        [SerializeField] private float m_maxPitch = 60f;

        [Header("Aim")]
        [SerializeField] private Vector3 m_defaultOffset = new(0f, 0f, -4f);
        [SerializeField] private Vector3 m_aimOffset = new(0.5f, -0.3f, -2f);
        [SerializeField] private float m_defaultFov = 60f;
        [SerializeField] private float m_aimFov = 45f;
        [SerializeField] private float m_aimSpeed = 10f;

        private PlayerInputActions m_actions;
        private Vector2 m_lookInput;
        private float m_pitch;
        private bool m_isAiming;

        public bool IsAiming => m_isAiming;
        public Vector3 Forward => m_camera.transform.forward;

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Look.performed += ctx => m_lookInput = ctx.ReadValue<Vector2>();
            m_actions.Player.Look.canceled += _ => m_lookInput = Vector2.zero;

            m_actions.Player.Aim.performed += _ => m_isAiming = true;
            m_actions.Player.Aim.canceled += _ => m_isAiming = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; 
        }

        private void LateUpdate()
        {
            HandleRotation();
            HandlePosition();

            Vector3 targetOffset = m_isAiming ? m_aimOffset : m_defaultOffset;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetOffset,
                Time.deltaTime * m_aimSpeed
            );
        }


       public void SetAiming(bool isAiming)
        {
            m_isAiming = isAiming;
        }

        private void HandleRotation()
        {
            float mouseX = m_lookInput.x * m_sensitivity;
            float mouseY = m_lookInput.y * m_sensitivity;

            m_pitch -= mouseY;
            m_pitch = Mathf.Clamp(m_pitch, m_minPitch, m_maxPitch);

            m_cameraRoot.localRotation = Quaternion.Euler(m_pitch, 0f, 0f);
            m_player.Rotate(Vector3.up * mouseX);
        }

        private void HandlePosition()
        {
            Vector3 targetOffset = m_isAiming ? m_aimOffset : m_defaultOffset;
            float targetFov = m_isAiming ? m_aimFov : m_defaultFov;

            m_camera.transform.localPosition = Vector3.Lerp(
                m_camera.transform.localPosition,
                targetOffset,
                Time.deltaTime * m_aimSpeed
            );

            m_camera.fieldOfView = Mathf.Lerp(
                m_camera.fieldOfView,
                targetFov,
                Time.deltaTime * m_aimSpeed
            );
        }
    }
}
