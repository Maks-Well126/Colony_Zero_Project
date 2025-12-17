using NUnit.Framework.Constraints;
using UnityEngine;


namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_targetPlayer;
        [SerializeField] private Transform m_pivotCamera;  

        [Header("Settings")]
        [SerializeField] private float m_sensitivity = 3f;
        [SerializeField] private float m_minPitch = -30f;
        [SerializeField] private float m_maxPitch = 60f;
        [SerializeField] private float m_distance = 4f;

        private PlayerInputActions m_actions;
        private Vector2 m_lookInput;

        private float m_yaw;
        private float m_pitch;

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Look.performed += ctx => m_lookInput = ctx.ReadValue<Vector2>();

            m_actions.Player.Look.canceled += ctx => m_lookInput = Vector2.zero;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; 
        }

        private void LateUpdate()
        {
            HandleRotation();
            HandlePosition();
        }

        private void HandleRotation()
        {
            m_yaw += m_lookInput.x * m_sensitivity;
            m_pitch -= m_lookInput.y * m_sensitivity;
            m_pitch = Mathf.Clamp(m_pitch, m_minPitch, m_maxPitch);

            // Поворот игрока (Yaw)
            m_targetPlayer.rotation = Quaternion.Euler(0f, m_yaw, 0f);

            // Поворот камеры вверх/вниз
            m_pivotCamera.localRotation = Quaternion.Euler(m_pitch, 0f, 0f);
        }

        private void HandlePosition()
        {
            transform.position = m_pivotCamera.position - transform.forward * m_distance;
        }


        public Vector3 Forward => Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        public Vector3 Right => Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
    }
}
