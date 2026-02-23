using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private PlayerController m_playerController;
        [SerializeField] private PlayerCameraController m_cameraController;
        [SerializeField] private WeaponSystem m_weaponSystem;
        [SerializeField] private WeaponUIController m_weaponUI;

        private PlayerInputActions m_actions;
        private PlayerState m_currentState;

        private float m_lastShootTime;

        private void Awake()
        {
            m_actions = new PlayerInputActions();
            m_actions.Player.Enable();

            m_actions.Player.Shoot.performed += _ => TryShoot();
            m_actions.Player.Reload.performed += _ => TryReload();
        }

        private void TryShoot()
        {
            WeaponInstance instance = m_weaponSystem.GetCurrentWeaponInstance();
            if (instance == null)
                return;

            WeaponConfig weapon = instance.Config;

            if (m_playerController.CurrentState != PlayerState.Aiming)
                return;

            if (Time.time < m_lastShootTime + weapon.FireRate)
                return;

            if (instance.CurrentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            m_lastShootTime = Time.time;

            m_playerController.SetState(PlayerState.Shooting);
            Shoot();
        }

        private void Shoot()
        {
            WeaponInstance instance = m_weaponSystem.GetCurrentWeaponInstance();
            if (instance == null)
                return;

            WeaponConfig weapon = instance.Config;

            instance.ConsumeAmmo();

            if (Physics.Raycast(
                m_camera.transform.position,
                m_camera.transform.forward,
                out RaycastHit hit,
                weapon.Range))
            {
                if (hit.collider.TryGetComponent(out HealthComponent health))
                {
                    health.TakeDamage(weapon.Damage);
                }
            }

            m_cameraController.ApplyRecoil(
                weapon.RecoilX,
                weapon.RecoilY,
                weapon.RecoilRecoverySpeed
            );

            SpawnMuzzleFlash();
            m_playerController.TriggerShootAnimation();

            if (weapon.ShootSound != null && instance.MuzzlePoint != null)
            {
                AudioSource.PlayClipAtPoint(
                    weapon.ShootSound,
                    instance.MuzzlePoint.position
                );
            }

            if (instance.CurrentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
            else
            {
                m_playerController.SetState(PlayerState.Aiming);
            }

            m_weaponUI.UpdateAmmo(
                instance.CurrentAmmo,
                weapon.MagazineSize
            );

        }

        private void TryReload()
        {
            WeaponInstance instance = m_weaponSystem.GetCurrentWeaponInstance();
            if (instance == null)
                return;

            if (m_playerController.CurrentState == PlayerState.Reloading)
                return;

            if (instance.CurrentAmmo == instance.Config.MagazineSize)
                return;

            StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            WeaponInstance instance = m_weaponSystem.GetCurrentWeaponInstance();
            if (instance == null)
                yield break;

            m_playerController.SetState(PlayerState.Reloading);

            yield return new WaitForSeconds(instance.Config.ReloadTime);

            instance.Reload();

            m_playerController.SetState(PlayerState.Idle);

            m_weaponUI.UpdateAmmo(
                instance.CurrentAmmo,
                instance.Config.MagazineSize
            );
            
        }

        private void SpawnMuzzleFlash()
        {
            WeaponInstance instance = m_weaponSystem.GetCurrentWeaponInstance();
            if (instance == null)
                return;

            if (instance.Config.MuzzleFlashPrefab == null)
                return;

            if (instance.MuzzlePoint == null)
                return;

            GameObject flash = Instantiate(
                instance.Config.MuzzleFlashPrefab,
                instance.MuzzlePoint.position,
                instance.MuzzlePoint.rotation,
                instance.MuzzlePoint
            );

            Destroy(flash, instance.Config.TimeMuzzle);
        }
    }
}