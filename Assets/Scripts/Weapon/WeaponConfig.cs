using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Xlab/Data/WeaponConfig")]
public sealed class WeaponConfig : ScriptableObject
{
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

    public float TimeMuzzle => m_timeMuzzle;
    public float RecoilX => m_recoilX;
    public float RecoilY => m_recoilY;
    public float RecoilRecoverySpeed => m_recoilRecoverySpeed;


    public float Damage => m_damage;
    public float FireRate => m_fireRate;
    public float Range => m_range;

    public GameObject MuzzleFlashPrefab => m_muzzleFlashPrefab;
    public AudioClip ShootSound => m_shootSound;
}
