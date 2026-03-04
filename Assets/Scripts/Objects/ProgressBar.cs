// using UnityEngine;
// using UnityEngine.UI;

// [RequireComponent(typeof(CanvasGroup))]
// public sealed class ProgressBar : MonoBehaviour
// {
//     [SerializeField] private Image m_fillImage;
//     [SerializeField] private float m_fadeSpeed = 8f;

//     private CanvasGroup m_canvasGroup;
//     private bool m_isVisible;
//     private float m_targetAlpha;

//     public bool IsVisible => m_isVisible;

//     private void Awake()
//     {
//         m_canvasGroup = GetComponent<CanvasGroup>();

//         if (m_fillImage == null)
//             m_fillImage = GetComponentInChildren<Image>();

//         Initialize();
//     }

//     private void Initialize()
//     {
//         m_fillImage.type = Image.Type.Filled;
//         m_fillImage.fillAmount = 0f;

//         m_canvasGroup.alpha = 0f;
//         m_targetAlpha = 0f;
//         m_isVisible = false;
//     }

//     private void Update()
//     {
//         // Плавный fade
//         if (Mathf.Abs(m_canvasGroup.alpha - m_targetAlpha) > 0.01f)
//         {
//             m_canvasGroup.alpha = Mathf.Lerp(
//                 m_canvasGroup.alpha,
//                 m_targetAlpha,
//                 Time.deltaTime * m_fadeSpeed
//             );
//         }
//     }

//     public void Show()
//     {
//         if (m_isVisible) return;

//         m_isVisible = true;
//         m_fillImage.fillAmount = 0f;
//         m_targetAlpha = 1f;
//     }

//     public void Hide()
//     {
//         if (!m_isVisible) return;

//         m_isVisible = false;
//         m_targetAlpha = 0f;
//     }

//     public void SetProgress(float value)
//     {
//         if (!m_isVisible) return;

//         m_fillImage.fillAmount = Mathf.Clamp01(value);
//     }

//     public void ResetProgress()
//     {
//         m_fillImage.fillAmount = 0f;
//     }
// }