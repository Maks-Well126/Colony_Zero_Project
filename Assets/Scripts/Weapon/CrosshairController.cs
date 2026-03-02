using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;
        [SerializeField] private Image m_crosshairImage;

        [SerializeField] private Color m_defaultColor = Color.white;
        [SerializeField] private Color m_enemyColor = Color.red;

        private void Awake()
        {
            m_root.SetActive(false);
            m_crosshairImage.color = m_defaultColor;
        }

        public void SetVisible(bool value)
        {
            m_root.SetActive(value);
        }

        public void SetEnemyTarget(bool isEnemy)
        {
            m_crosshairImage.color = isEnemy ? m_enemyColor : m_defaultColor;
        }
    }
}