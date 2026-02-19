using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private WeaponConfig m_weaponConfig;
        [SerializeField] private Camera m_camera;
        [SerializeField] private Transform m_muzzlePoint;

        private float m_lastShootTime;
        private PlayerInputActions m_actions;
        private PlayerCameraController m_cameraController;
        

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();
            m_actions.Player.Shoot.performed += _ => TryShoot();

            m_cameraController = m_camera.GetComponent<PlayerCameraController>();
        }

        private void TryShoot()
        {
            if (Time.time < m_lastShootTime + m_weaponConfig.FireRate)
                return;

            if (!m_cameraController.IsAiming)
                return;

            m_lastShootTime = Time.time;
            Shoot();
        }

        private void Shoot()
        {
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

            m_cameraController.ApplyRecoil(
                m_weaponConfig.RecoilX,
                m_weaponConfig.RecoilY,
                m_weaponConfig.RecoilRecoverySpeed
            );

            SpawnMuzzleFlash();


            if (m_weaponConfig.ShootSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    m_weaponConfig.ShootSound,
                    m_muzzlePoint.position
                );
            }
        }

        private void SpawnMuzzleFlash()
        {
            if (m_weaponConfig.MuzzleFlashPrefab == null || m_muzzlePoint == null)
                return;

            GameObject flash = Instantiate(
                m_weaponConfig.MuzzleFlashPrefab,
                m_muzzlePoint.position,
                m_muzzlePoint.rotation
            );

            flash.transform.SetParent(m_muzzlePoint);

            Destroy(flash, m_weaponConfig.TimeMuzzle); 
        }
    }
}
