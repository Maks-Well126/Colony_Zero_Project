using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private PlayerCameraController m_camera;
        [SerializeField] private float m_distance = 200f;
        [SerializeField] private LayerMask m_hitMask;

        private PlayerInputActions m_actions;

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Shoot.performed += _ => Shoot();
        }

        private void Shoot()
        {
            Ray ray = new Ray(
                m_camera.transform.position,
                m_camera.Forward
            );

            if (Physics.Raycast(ray, out RaycastHit hit, m_distance, m_hitMask))
            {
                Debug.Log($"Hit: {hit.collider.name}");
            }
        }
    }
}
