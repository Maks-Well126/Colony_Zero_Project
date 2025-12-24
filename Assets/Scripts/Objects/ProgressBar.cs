using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("Prefab Reference")]
    [SerializeField] private GameObject m_progressBarPrefab;

    [Header("Positioning")]
    [SerializeField] private Vector3 m_offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private bool m_faceCamera = true;

    [Header("Completion Settings")]
    [SerializeField] private float m_hideDelay = 0.3f;

    private Canvas m_progressCanvas;
    private Slider m_progressSlider;
    private Image m_fillImage;
    private Camera m_mainCamera;
    private Transform m_targetTransform;

    public void SetPrefab(GameObject prefab)
    {
        m_progressBarPrefab = prefab;
    }

    public void Initialize(Transform parent)
    {
        if (m_progressBarPrefab == null)
        {
            return;
        }

        m_targetTransform = parent;
        m_mainCamera = Camera.main;

        CreateProgressBarFromPrefab();
        SetVisible(false);
    }

    private void CreateProgressBarFromPrefab()
    {
        GameObject canvasObj = Instantiate(m_progressBarPrefab,
            m_targetTransform.position + m_offset,
            Quaternion.identity);

        canvasObj.transform.SetParent(m_targetTransform);
        canvasObj.name = $"{m_targetTransform.name}_ProgressCanvas";

        m_progressCanvas = canvasObj.GetComponent<Canvas>();
        if (m_progressCanvas == null)
        {
            m_progressCanvas = canvasObj.AddComponent<Canvas>();
            m_progressCanvas.renderMode = RenderMode.WorldSpace;
        }

        m_progressCanvas.worldCamera = m_mainCamera;

        SetupSliderComponents(canvasObj);
    }

    private void SetupSliderComponents(GameObject canvasObj)
    {
        m_progressSlider = canvasObj.GetComponentInChildren<Slider>();
        if (m_progressSlider == null)
        {
            return;
        }

        m_progressSlider.minValue = 0;
        m_progressSlider.maxValue = 1;
        m_progressSlider.value = 0;

        if (m_progressSlider.fillRect != null)
        {
            m_fillImage = m_progressSlider.fillRect.GetComponent<Image>();
        }
    }

    public void StartProgress()
    {
        SetVisible(true);
        ResetProgress();
    }

    public void UpdateProgress(float progress)
    {
        SetProgress(progress);
        UpdateFillColor(progress);
    }

    public void CompleteProgress()
    {
        SetProgress(1f);
        SetFillColor(Color.green);
        ScheduleHide();
    }

    public void CancelProgress()
    {
        HideProgress();
    }

    public void SetHideDelay(float delay)
    {
        m_hideDelay = Mathf.Max(0, delay);
    }

    private void ResetProgress()
    {
        SetProgress(0f);
        SetFillColor(Color.red);
    }

    private void SetProgress(float value)
    {
        if (m_progressSlider != null)
        {
            m_progressSlider.value = Mathf.Clamp01(value);
        }
    }

    private void UpdateFillColor(float progress)
    {
        if (m_fillImage == null) return;

        if (progress < 0.5f)
        {
            float t = progress * 2f;
            m_fillImage.color = Color.Lerp(Color.red, Color.yellow, t);
        }
        else
        {
            float t = (progress - 0.5f) * 2f;
            m_fillImage.color = Color.Lerp(Color.yellow, Color.green, t);
        }
    }

    private void SetFillColor(Color color)
    {
        if (m_fillImage != null)
        {
            m_fillImage.color = color;
        }
    }

    private void SetVisible(bool visible)
    {
        if (m_progressCanvas != null)
        {
            m_progressCanvas.gameObject.SetActive(visible);
        }
    }

    private void ScheduleHide()
    {
        Invoke(nameof(HideProgress), m_hideDelay);
    }

    private void HideProgress()
    {
        SetVisible(false);
    }

    private void Update()
    {
        if (ShouldFaceCamera())
        {
            FaceCamera();
        }
    }

    private bool ShouldFaceCamera()
    {
        return m_faceCamera &&
               m_progressCanvas != null &&
               m_progressCanvas.gameObject.activeSelf &&
               m_mainCamera != null;
    }

    private void FaceCamera()
    {
        Vector3 direction = m_progressCanvas.transform.position - m_mainCamera.transform.position;
        m_progressCanvas.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (m_progressCanvas != null)
        {
            Destroy(m_progressCanvas.gameObject);
        }
    }
}