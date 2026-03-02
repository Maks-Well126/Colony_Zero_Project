using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [SerializeField] private Transform m_muzzlePoint;
    private AudioSource m_audioSource;
    public Transform MuzzlePoint => m_muzzlePoint;

    public WeaponConfig Config { get; private set; }
    public int CurrentAmmo { get; private set; }
    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(WeaponConfig config)
    {
        Config = config;
        CurrentAmmo = config.MagazineSize;
    }

    public void ConsumeAmmo()
    {
        if (CurrentAmmo > 0)
            CurrentAmmo--;
    }
    public void PlayReloadSound()
    {
        if (Config.ReloadSound == null)
            return;

        if (m_audioSource != null)
            m_audioSource.PlayOneShot(Config.ReloadSound);
    }

    public void Reload()
    {

        CurrentAmmo = Config.MagazineSize;
    }
}