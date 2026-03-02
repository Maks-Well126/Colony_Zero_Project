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
        m_actions.Player.SwitchWeapon.performed += OnSwitchWeapon;
    }

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        SwitchWeapon();
    }

    private void OnDestroy()
    {
        m_actions.Player.SwitchWeapon.performed -= OnSwitchWeapon;
    }

    private void Start()
    {
        m_primaryInstance = CreateWeapon(m_primaryWeapon);
        m_secondaryInstance = CreateWeapon(m_secondaryWeapon);

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
        if (weapon == null)
            return;

        if (m_currentWeaponInstance != null)
            m_currentWeaponInstance.gameObject.SetActive(false);

        m_currentWeaponInstance = weapon;

        if (m_currentWeaponInstance == null)
            return;

        m_currentWeaponInstance.gameObject.SetActive(true);

        if (m_weaponUI != null)
        {
            m_weaponUI.UpdateAmmo(
                m_currentWeaponInstance.CurrentAmmo,
                m_currentWeaponInstance.Config.MagazineSize
            );

            var inactive = GetInactiveWeapon();
            if (inactive != null)
            {
                m_weaponUI.UpdateWeaponUI(
                    m_currentWeaponInstance.Config,
                    inactive.Config
                );
            }
        }
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