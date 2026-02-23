using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponConfig m_primaryWeapon;
    [SerializeField] private WeaponConfig m_secondaryWeapon;

    [SerializeField] private Transform m_weaponHolder;
    [SerializeField] private WeaponUIController m_weaponUI;

    private PlayerInputActions m_actions;

    private WeaponInstance m_primaryInstance;
    private WeaponInstance m_secondaryInstance;

    private WeaponInstance m_currentWeaponInstance;

    private void Awake()
    {
        m_actions = new PlayerInputActions();
        m_actions.Player.Enable();
        m_actions.Player.SwitchWeapon.performed += _ => SwitchWeapon();
    }

    private void Start()
    {
        // Создаём оба оружия ОДИН РАЗ
        m_primaryInstance = CreateWeapon(m_primaryWeapon);
        m_secondaryInstance = CreateWeapon(m_secondaryWeapon);

        // Активируем только основное
        SetActiveWeapon(m_primaryInstance);
    }

    private WeaponInstance CreateWeapon(WeaponConfig config)
    {
        GameObject obj = Instantiate(config.WeaponPrefab, m_weaponHolder);
        WeaponInstance instance = obj.GetComponent<WeaponInstance>();

        instance.Initialize(config);

        obj.SetActive(false);

        return instance;
    }

    private void SetActiveWeapon(WeaponInstance weapon)
    {
        if (m_currentWeaponInstance != null)
            m_currentWeaponInstance.gameObject.SetActive(false);

        m_currentWeaponInstance = weapon;
        m_currentWeaponInstance.gameObject.SetActive(true);

        m_weaponUI.UpdateAmmo(
            m_currentWeaponInstance.CurrentAmmo,
            m_currentWeaponInstance.Config.MagazineSize
        );

        m_weaponUI.UpdateWeaponUI(
            m_currentWeaponInstance.Config,
            GetInactiveWeapon().Config
        );
    }

    private WeaponInstance GetInactiveWeapon()
    {
        return m_currentWeaponInstance == m_primaryInstance
            ? m_secondaryInstance
            : m_primaryInstance;
    }

    private void SwitchWeapon()
    {
        if (m_currentWeaponInstance == m_primaryInstance)
            SetActiveWeapon(m_secondaryInstance);
        else
            SetActiveWeapon(m_primaryInstance);
    }

    public WeaponInstance GetCurrentWeaponInstance()
    {
        return m_currentWeaponInstance;
    }
}