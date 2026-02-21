using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private WeaponConfig m_weaponConfig;
        [SerializeField] private Camera m_camera;
        [SerializeField] private Transform m_muzzlePoint;
        [SerializeField] private PlayerController m_playerController;
        [SerializeField] private PlayerCameraController m_cameraController;

        private PlayerInputActions m_actions;

        private float m_lastShootTime;
        private int m_currentAmmo;

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Shoot.performed += _ => TryShoot();
            m_actions.Player.Reload.performed += _ => TryReload();
            
        }

        private void Start()
        {
            m_currentAmmo = m_weaponConfig.MagazineSize;
        }

        private void TryShoot()
        {
            if (m_playerController.CurrentState != PlayerController.PlayerState.Aiming)
                return;

            if (Time.time < m_lastShootTime + m_weaponConfig.FireRate)
                return;

            if (m_currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            m_lastShootTime = Time.time;

            m_playerController.SetState(PlayerController.PlayerState.Shooting);
            Shoot();
        }

        private void Shoot()
        {
            m_currentAmmo--;

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
            m_playerController.TriggerShootAnimation();
            if (m_weaponConfig.ShootSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    m_weaponConfig.ShootSound,
                    m_muzzlePoint.position
                );
            }
          

            if (m_currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
            else
            {
                m_playerController.SetState(PlayerController.PlayerState.Aiming);
            }
        }

        private void TryReload()
        {
            if (m_playerController.CurrentState == PlayerController.PlayerState.Reloading)
                return;

            if (m_currentAmmo == m_weaponConfig.MagazineSize)
                return;

            StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            m_playerController.SetState(PlayerController.PlayerState.Reloading);

            yield return new WaitForSeconds(m_weaponConfig.ReloadTime);

            m_currentAmmo = m_weaponConfig.MagazineSize;

            m_playerController.SetState(PlayerController.PlayerState.Idle);
        }

        private void SpawnMuzzleFlash()
        {
            if (m_weaponConfig.MuzzleFlashPrefab == null || m_muzzlePoint == null)
                return;

            GameObject flash = Instantiate(
                m_weaponConfig.MuzzleFlashPrefab,
                m_muzzlePoint.position,
                m_muzzlePoint.rotation,
                m_muzzlePoint
            );

            Destroy(flash, m_weaponConfig.TimeMuzzle);
        }
    }
}