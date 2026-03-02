using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WeaponUIController : MonoBehaviour
{
    [SerializeField] private Image m_activeWeaponIcon;
    [SerializeField] private Image m_inactiveWeaponIcon;
    [SerializeField] private TMP_Text m_ammoText;

    public void UpdateWeaponUI(WeaponConfig active, WeaponConfig inactive)
    {
        m_activeWeaponIcon.sprite = active.WeaponIcon;
        m_inactiveWeaponIcon.sprite = inactive.WeaponIcon;
    }

    public void UpdateAmmo(int current, int max)
    {
        m_ammoText.text = current + " / ∞";
    }
}