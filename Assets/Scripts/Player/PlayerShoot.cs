using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Audio;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private PlayerController m_playerController;
        [SerializeField] private PlayerCameraController m_cameraController;
        [SerializeField] private WeaponSystem m_weaponSystem;
        [SerializeField] private WeaponUIController m_weaponUI;
        [SerializeField] private AudioMixerGroup masterGroup; // присвоить Master из AudioMixer

        private PlayerInputActions m_actions;
        private PlayerState m_currentState;
        private bool m_isReloading;
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
                if (hit.collider.TryGetComponent(out HealingPlant plant))
                {
                    plant.Hit();
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
                PlayClipAtPointMaster(
                    weapon.ShootSound,
                    instance.MuzzlePoint.position,
                    weapon.ShootVolume
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
            if (m_isReloading)
                yield break;

            WeaponInstance instance = m_weaponSystem.GetCurrentWeaponInstance();
            if (instance == null)
                yield break;

            m_isReloading = true;

            m_playerController.SetState(PlayerState.Reloading);

            float delay = instance.Config.ReloadSoundDelay;
            float totalTime = instance.Config.ReloadTime;

            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            if (instance.Config.ReloadSound != null)
            {
                PlayClipAtPointMaster(
                    instance.Config.ReloadSound,
                    transform.position,
                    instance.Config.ReloadVolume
                );
            }

            float remainingTime = Mathf.Max(0f, totalTime - delay);
            if (remainingTime > 0f)
                yield return new WaitForSeconds(remainingTime);

            instance.Reload();

            m_playerController.SetState(PlayerState.Idle);

            m_weaponUI.UpdateAmmo(
                instance.CurrentAmmo,
                instance.Config.MagazineSize
            );

            m_isReloading = false;
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
        private void PlayClipAtPointMaster(AudioClip clip, Vector3 position, float volume)
        {
            if (clip == null) return;

            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.outputAudioMixerGroup = masterGroup; // подключаем к Master
            aSource.spatialBlend = 1f; // 3D звук
            aSource.volume = volume;   // индивидуальная громкость через инспектор
            aSource.Play();
            Destroy(tempGO, clip.length);
        }

        private void OnDestroy()
        {
            if (m_actions != null)
            {
                m_actions.Player.Shoot.performed -= _ => TryShoot();
                m_actions.Player.Reload.performed -= _ => TryReload();
                m_actions.Dispose();
            }
        }
    }
}