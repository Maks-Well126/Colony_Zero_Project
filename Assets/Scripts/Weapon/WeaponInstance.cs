using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [SerializeField] private Transform m_muzzlePoint;

    public Transform MuzzlePoint => m_muzzlePoint;
}