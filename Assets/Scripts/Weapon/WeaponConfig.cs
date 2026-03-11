using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Xlab/Data/WeaponConfig")]
public sealed class WeaponConfig : ScriptableObject
{
    [Header("Presentation")]
    [SerializeField] private GameObject m_weaponPrefab;
    [SerializeField] private Sprite m_weaponIcon;

    [Header("Damage")]
    [SerializeField][Min(1)] private float m_damage = 20f;
    [SerializeField][Min(0)] private float m_fireRate = 0.2f;

    [Header("Range")]
    [SerializeField][Min(1)] private float m_range = 100f;

    [Header("Visual")]
    [SerializeField] private GameObject m_muzzleFlashPrefab;
    [SerializeField] private float m_timeMuzzle = 0.2f;
    [SerializeField] private AudioClip m_shootSound;

    [Header("Recoil")]
    [SerializeField] private float m_recoilX = 2f;
    [SerializeField] private float m_recoilY = 1f;
    [SerializeField] private float m_recoilRecoverySpeed = 5f;

    [Header("Ammo")]
    [SerializeField] private int m_magazineSize = 30;
    [SerializeField] private float m_reloadTime = 1.5f;
    [SerializeField] private AudioClip m_reloadSound;
    [SerializeField][Min(0f)] private float m_reloadSoundDelay = 0.3f;
    
    [Header("Audio Volume")]
    [SerializeField][Range(0f, 1f)] private float m_shootVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float m_reloadVolume = 1f;

    public float ShootVolume => m_shootVolume;
    public float ReloadVolume => m_reloadVolume;

    public GameObject WeaponPrefab => m_weaponPrefab;
    public Sprite WeaponIcon => m_weaponIcon;

    public int MagazineSize => m_magazineSize;
    public float ReloadTime => m_reloadTime;
    public AudioClip ReloadSound => m_reloadSound;
    public float ReloadSoundDelay => m_reloadSoundDelay;

    public float TimeMuzzle => m_timeMuzzle;
    public GameObject MuzzleFlashPrefab => m_muzzleFlashPrefab;
    public AudioClip ShootSound => m_shootSound;

    public float RecoilX => m_recoilX;
    public float RecoilY => m_recoilY;
    public float RecoilRecoverySpeed => m_recoilRecoverySpeed;

    public float Damage => m_damage;
    public float FireRate => m_fireRate;
    public float Range => m_range;
}