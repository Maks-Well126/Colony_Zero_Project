using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [SerializeField] private Transform m_muzzlePoint;

    private int m_currentAmmo;
    private WeaponConfig m_config;

    public Transform MuzzlePoint => m_muzzlePoint;
    public int CurrentAmmo => m_currentAmmo;
    public WeaponConfig Config => m_config;

    public void Initialize(WeaponConfig config)
    {
        m_config = config;
        m_currentAmmo = config.MagazineSize;
    }

    public void ConsumeAmmo()
    {
        m_currentAmmo--;
    }

    public void Reload()
    {
        m_currentAmmo = m_config.MagazineSize;
    }

}