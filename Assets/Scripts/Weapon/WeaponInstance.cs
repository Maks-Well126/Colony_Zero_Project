using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [SerializeField] private Transform m_muzzlePoint;
    public Transform MuzzlePoint => m_muzzlePoint;

    public WeaponConfig Config { get; private set; }
    public int CurrentAmmo { get; private set; }

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

    public void Reload()
    {
        CurrentAmmo = Config.MagazineSize;
    }

    
}