using UnityEngine;


namespace Player
    {
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;

        private void Awake()
        {
            m_root.SetActive(false);
        }

        public void SetVisible(bool value)
        {
            m_root.SetActive(value);
        }
    }
    }

