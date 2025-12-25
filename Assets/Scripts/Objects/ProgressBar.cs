using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Vector3 m_offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float m_scale = 0.01f;

    private GameObject m_instance;
    private Slider m_slider;
    private Camera m_camera;
    private Transform m_target;
    private Canvas m_canvas;

    public void Show(Transform target)
    {
        if (m_instance != null) return;

        m_target = target;
        m_camera = Camera.main;

        m_instance = Instantiate(m_prefab);
        m_instance.transform.position = target.position + m_offset;
        m_instance.transform.localScale = Vector3.one * m_scale;

        m_slider = m_instance.GetComponentInChildren<Slider>();
        if (m_slider != null)
            m_slider.value = 0f;

        m_canvas = m_instance.GetComponentInChildren<Canvas>();
        if (m_canvas != null)
        {
            m_canvas.renderMode = RenderMode.WorldSpace;
            m_canvas.worldCamera = m_camera;
        }
    }

    public void SetProgress(float value)
    {
        if (m_slider != null)
            m_slider.value = Mathf.Clamp01(value);
    }

    public void Hide()
    {
        if (m_instance != null)
            Destroy(m_instance);
    }

    private void LateUpdate()
    {
        if (m_canvas == null || m_camera == null)
            return;

        Vector3 direction = m_camera.transform.position - m_canvas.transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
            m_canvas.transform.forward = direction;
    }
}
