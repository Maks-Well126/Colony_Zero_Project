using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private WeaponConfig m_weaponConfig;
        [SerializeField] private Camera m_camera;
        [SerializeField] private AudioSource m_audioSource;


        private float m_lastShootTime;
        private PlayerInputActions m_actions;

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();
            m_actions.Player.Shoot.performed += _ => TryShoot();
        }

        private void TryShoot()
        {
            if (Time.time < m_lastShootTime + m_weaponConfig.FireRate)
                return;

            m_lastShootTime = Time.time;
            Shoot();
        }

        private void Shoot()
        {
            PlayShootSound();

            if (Physics.Raycast(
                m_camera.transform.position,
                m_camera.transform.forward,
                out RaycastHit hit,
                m_weaponConfig.Range))
            {
                if (hit.collider.TryGetComponent(out HealthComponent health))
                {
                    health.TakeDamage(m_weaponConfig.Damage);
                }
            }
        }

        private void PlayShootSound()
        {
            if (m_audioSource == null || m_weaponConfig.ShootSound == null)
                return;

            m_audioSource.PlayOneShot(m_weaponConfig.ShootSound);
        }
    }
}