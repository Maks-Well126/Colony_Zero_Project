using UnityEngine;
using TMPro;

public class HintMarker : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform m_target;              // Цель
    [SerializeField] private Transform m_player;              // Игрок
    [SerializeField] private Camera m_mainCamera;             // Камера

    [Header("Marker Prefab")]
    [SerializeField] private RectTransform m_markerPrefab;    // Префаб с Image + TMP_Text
    [SerializeField] private RectTransform m_markerUI;       // Копия префаба
    [SerializeField] private TMP_Text m_distanceTMP;         // Текст внутри префаба

    [Header("Marker Offset")]
    [SerializeField] private Vector3 m_offset = new Vector3(0, 2, 0);

    [Header("Scale Settings")]
    [SerializeField] private float m_maxSize = 1.5f;
    [SerializeField] private float m_minSize = 0.5f;
    [SerializeField] private float m_distanceScaleFactor = 0.05f;

    [Header("Screen Edge Settings")]
    [SerializeField] private float m_edgeBuffer = 50f;

    [Header("Hide Marker Settings")]
    [SerializeField] private float m_hideDistance = 2.0f;
    [SerializeField] private float m_screenOverlapThreshold = 30f;

    void Start()
    {
        if (m_markerPrefab == null)
        {
            Debug.LogError("Marker Prefab is not assigned!");
            return;
        }

        m_markerPrefab.gameObject.SetActive(false);

        m_markerUI = Instantiate(m_markerPrefab, m_markerPrefab.parent);
        m_markerUI.gameObject.SetActive(true);

        m_distanceTMP = m_markerUI.GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        if (m_target == null || m_markerUI == null || m_mainCamera == null || m_player == null)
            return;

        Vector3 worldPos = m_target.position + m_offset;
        Vector3 screenPos = m_mainCamera.WorldToScreenPoint(worldPos);
        float distanceToTarget = Vector3.Distance(m_player.position, m_target.position);

        Vector3 playerScreenPos = m_mainCamera.WorldToScreenPoint(m_player.position);
        bool closeOnScreen = Vector2.Distance(playerScreenPos, screenPos) < m_screenOverlapThreshold;
        if (distanceToTarget < m_hideDistance || closeOnScreen)
        {
            m_markerUI.gameObject.SetActive(false);
            return;
        }

        bool isBehindCamera = screenPos.z < 0;
        if (isBehindCamera)
        {
            screenPos *= -1;
            screenPos.z = 0.01f;
        }

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector2 screenCenter = new Vector2(screenWidth / 2f, screenHeight / 2f);
        
        bool isOnScreen = screenPos.z > 0 &&
                          screenPos.x >= 0 && screenPos.x <= screenWidth &&
                          screenPos.y >= 0 && screenPos.y <= screenHeight;
      
        Vector2 clampedScreenPos;
        if (isOnScreen)
        {
            clampedScreenPos = screenPos;
        }
        else
        {
            Vector2 fromCenter = new Vector2(screenPos.x, screenPos.y) - screenCenter;
            Vector2 dir = fromCenter.normalized;

            float halfWidth = (screenWidth / 2f) - m_edgeBuffer;
            float halfHeight = (screenHeight / 2f) - m_edgeBuffer;

            float slope = dir.y / dir.x;
            Vector2 edgeHit = screenCenter;

            if (Mathf.Abs(slope) < (halfHeight / halfWidth))
            {
                edgeHit.x += Mathf.Sign(dir.x) * halfWidth;
                edgeHit.y += Mathf.Sign(dir.x) * halfWidth * slope;
            }
            else
            {
                edgeHit.y += Mathf.Sign(dir.y) * halfHeight;
                edgeHit.x += Mathf.Sign(dir.y) * halfHeight / slope;
            }

            clampedScreenPos = new Vector2(
                Mathf.Clamp(edgeHit.x, m_edgeBuffer, screenWidth - m_edgeBuffer),
                Mathf.Clamp(edgeHit.y, m_edgeBuffer, screenHeight - m_edgeBuffer)
            );
        }

        m_markerUI.position = clampedScreenPos;
        m_markerUI.gameObject.SetActive(true);

        float scale = Mathf.Clamp(1 / (distanceToTarget * m_distanceScaleFactor), m_minSize, m_maxSize);
        m_markerUI.localScale = Vector3.one * scale;

        if (!isOnScreen)
        {
            Vector2 directionToTarget = clampedScreenPos - screenCenter;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            m_markerUI.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            m_markerUI.rotation = Quaternion.identity;
        }

        if (m_distanceTMP != null)
        {
            m_distanceTMP.text = $"{Mathf.RoundToInt(distanceToTarget)}m";
        }
    }
}
