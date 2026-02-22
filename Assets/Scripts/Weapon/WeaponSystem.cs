using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponConfig m_primaryWeapon;
    [SerializeField] private WeaponConfig m_secondaryWeapon;

    [SerializeField] private Transform m_weaponHolder;

    [Header("UI")]
    [SerializeField] private WeaponUIController m_weaponUI;

    private PlayerInputActions m_actions;

    private WeaponConfig m_currentWeapon;
    private WeaponConfig m_inactiveWeapon;

    private GameObject m_currentWeaponObject;
    private WeaponInstance m_currentWeaponInstance;
    

    private void Awake()
    {
        m_actions = new PlayerInputActions();
        m_actions.Player.Enable();

        m_actions.Player.SwitchWeapon.performed += _ => SwitchWeapon();
    }

    private void Start()
    {
        EquipWeapon(m_primaryWeapon, m_secondaryWeapon);
    }


    private void EquipWeapon(WeaponConfig active, WeaponConfig inactive)
{
    m_currentWeapon = active;
    m_inactiveWeapon = inactive;

    if (m_currentWeaponObject != null)
        Destroy(m_currentWeaponObject);

    m_currentWeaponObject = Instantiate(
        m_currentWeapon.WeaponPrefab,
        m_weaponHolder
    );

    // Получаем WeaponInstance на новом объекте
    m_currentWeaponInstance = m_currentWeaponObject.GetComponent<WeaponInstance>();

    m_weaponUI.UpdateWeaponUI(
        m_currentWeapon,
        m_inactiveWeapon
    );
}

public WeaponInstance GetCurrentWeaponInstance()
{
    return m_currentWeaponInstance;
}

    private void SwitchWeapon()
    {
        EquipWeapon(m_inactiveWeapon, m_currentWeapon);
    }

    public WeaponConfig GetCurrentWeapon()
    {
        return m_currentWeapon;
    }
}
