using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private WeaponConfig m_weaponConfig;
        [SerializeField] private Camera m_camera;
        [SerializeField] private PlayerController m_playerController;
        [SerializeField] private PlayerCameraController m_cameraController;
        [SerializeField] private WeaponSystem m_weaponSystem;
        [SerializeField] private WeaponUIController m_weaponUI;

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

            m_currentAmmo = m_weaponSystem.GetCurrentWeapon().MagazineSize;
            m_weaponUI.UpdateAmmo(
                m_currentAmmo,
                m_weaponSystem.GetCurrentWeapon().MagazineSize
            );
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
                WeaponInstance weaponInstance = m_weaponSystem.GetCurrentWeaponInstance();
                if (weaponInstance != null && weaponInstance.MuzzlePoint != null)
                {
                    AudioSource.PlayClipAtPoint(
                        m_weaponConfig.ShootSound,
                        weaponInstance.MuzzlePoint.position
                    );
                }
            }


            if (m_currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
            else
            {
                m_playerController.SetState(PlayerController.PlayerState.Aiming);
            }

            m_weaponUI.UpdateAmmo(
                m_currentAmmo,
                m_weaponSystem.GetCurrentWeapon().MagazineSize
            );
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

            m_currentAmmo = m_weaponSystem.GetCurrentWeapon().MagazineSize;

            m_weaponUI.UpdateAmmo(
                m_currentAmmo,
                m_weaponSystem.GetCurrentWeapon().MagazineSize
            );
        }

        private void SpawnMuzzleFlash()
        {
            if (m_weaponConfig.MuzzleFlashPrefab == null)
                return;

            // Получаем текущий инстанс оружия
            WeaponInstance weaponInstance = m_weaponSystem.GetCurrentWeaponInstance();
            if (weaponInstance == null)
                return;

            Transform muzzle = weaponInstance.MuzzlePoint;
            if (muzzle == null)
                return;

            GameObject flash = Instantiate(
                m_weaponConfig.MuzzleFlashPrefab,
                muzzle.position,
                muzzle.rotation,
                muzzle
            );

            Destroy(flash, m_weaponConfig.TimeMuzzle);
        }
    }
}