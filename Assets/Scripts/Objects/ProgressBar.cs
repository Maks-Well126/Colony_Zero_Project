using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Vector3 m_offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float m_scale = 0.003f;
    [SerializeField] private bool m_faceCamera = true;
    [SerializeField] private float m_rotationSpeed = 10f;
    [SerializeField] private Vector2 m_canvasSize = new Vector2(200, 30);

    private GameObject m_instance;
    private Slider m_slider;
    private Canvas m_canvas;
    private Transform m_target;
    private Camera m_camera;
    private bool m_isActive;

    public void Show(Transform target)
    {
        if (m_isActive) Hide();

        m_target = target;
        m_isActive = true;
        if (m_camera == null) FindCamera();
        if (m_prefab == null || m_camera == null) return;

        CreateInstance();
        SetupComponents();
    }

    public void SetProgress(float value) => m_slider?.SetValue(value);
    public void Hide() => DestroyInstance();
    public bool IsActive => m_isActive;

    private void FindCamera() => m_camera = Camera.main;

    private void CreateInstance()
    {
        m_instance = Instantiate(m_prefab, m_target.position + m_offset, Quaternion.identity);
        m_instance.transform.SetParent(m_target);
        m_instance.transform.localPosition = m_offset;
        m_instance.transform.localScale = Vector3.one * m_scale;
    }

    private void SetupComponents()
    {
        m_slider = m_instance.GetComponentInChildren<Slider>();
        if (m_slider == null)
        {
            Debug.LogError("ProgressBar: Slider not found!");
            DestroyInstance();
            return;
        }

        m_slider.Setup(0, 1, 0);
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        m_canvas = m_instance.GetComponentInChildren<Canvas>();
        if (m_canvas == null) return;

        m_canvas.renderMode = RenderMode.WorldSpace;
        m_canvas.worldCamera = m_camera;

        var rectTransform = m_canvas.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = m_canvasSize;
            rectTransform.SetCenterPivot();
        }

        if (m_faceCamera) UpdateCanvasRotation();
    }

    private void UpdateCanvasRotation()
    {
        if (m_canvas == null || m_camera == null) return;

        var direction = m_camera.transform.position - m_canvas.transform.position;
        if (direction == Vector3.zero) return;

        m_canvas.transform.rotation = Quaternion.LookRotation(-direction);
    }

    private void DestroyInstance()
    {
        if (m_instance == null) return;

        Destroy(m_instance);
        m_instance = null;
        m_slider = null;
        m_canvas = null;
        m_target = null;
        m_isActive = false;
    }

    private void LateUpdate()
    {
        if (!m_isActive || !m_faceCamera || m_canvas == null || m_camera == null) return;

        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        if (m_target == null) return;
        m_instance.transform.position = m_target.position + m_offset;
    }

    private void UpdateRotation()
    {
        var direction = m_camera.transform.position - m_canvas.transform.position;
        if (direction == Vector3.zero) return;

        var targetRotation = Quaternion.LookRotation(-direction);
        m_canvas.transform.rotation = Quaternion.Slerp(
            m_canvas.transform.rotation,
            targetRotation,
            m_rotationSpeed * Time.deltaTime
        );
    }
}

// Extension методы для удобства
public static class ProgressBarExtensions
{
    public static void SetValue(this Slider slider, float value) => 
        slider.value = Mathf.Clamp01(value);
    
    public static void Setup(this Slider slider, float min, float max, float value)
    {
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
    }
    
    public static void SetCenterPivot(this RectTransform rectTransform)
    {
        rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = 
            new Vector2(0.5f, 0.5f);
    }
}