using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;
        [SerializeField] private Image m_crosshairImage;

        [SerializeField] private Color m_defaultColor = Color.white;

        private void Awake()
        {
            m_root.SetActive(false);
            m_crosshairImage.color = m_defaultColor;
        }

        public void SetVisible(bool value)
        {
            m_root.SetActive(value);
        }

        public void SetTargetColor(Color color)
        {
            m_crosshairImage.color = color;
        }
    }
}
