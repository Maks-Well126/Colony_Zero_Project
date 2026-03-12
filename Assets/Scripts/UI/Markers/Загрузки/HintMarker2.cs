using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintMarker2 : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform[] targets;
    private int currentTargetIndex = 0;
    public float switchDistance = 2f;

    [Header("Player and Camera")]
    public Transform player;
    public Camera mainCamera;
    public RectTransform markerPrefab; // Префаб с Image+Text
    private RectTransform markerUI;    // Копия префаба
    private TMP_Text distanceTMP;      // Текст дистанции внутри префаба

    public Vector3 offset = new Vector3(0, 2, 0);

    [Header("Scale Settings")]
    public float maxSize = 1.5f;
    public float minSize = 0.5f;
    public float distanceScaleFactor = 0.05f;

    [Header("Screen Edge Settings")]
    public float edgeBuffer = 50f;

    [Header("Hide Marker Settings")]
    public float hideDistance = 1.5f;
    public float screenOverlapThreshold = 30f;

    void Start()
    {
        if (markerPrefab == null)
        {
            Debug.LogError("Marker Prefab is not assigned!");
            return;
        }

        // Отключаем оригинал, создаём копию
        markerPrefab.gameObject.SetActive(false);
        markerUI = Instantiate(markerPrefab, markerPrefab.parent);
        markerUI.gameObject.SetActive(true);

        // Ищем TMP_Text внутри префаба
        distanceTMP = markerUI.GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        if (targets.Length == 0 || markerUI == null || mainCamera == null || player == null)
            return;

        Transform target = targets[currentTargetIndex];
        if (target == null) return;

        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        float distanceToTarget = Vector3.Distance(player.position, target.position);

        if (distanceToTarget < switchDistance && currentTargetIndex < targets.Length - 1)
        {
            currentTargetIndex++;
            return;
        }

        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(player.position);
        bool closeOnScreen = Vector2.Distance(playerScreenPos, screenPos) < screenOverlapThreshold;

        if (distanceToTarget < hideDistance || closeOnScreen)
        {
            markerUI.gameObject.SetActive(false);
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
            float slope = dir.y / dir.x;
            Vector2 edgePoint = screenCenter;

            if (Mathf.Abs(slope) < (screenHeight / screenWidth))
            {
                edgePoint.x = dir.x > 0 ? screenWidth - edgeBuffer : edgeBuffer;
                edgePoint.y = screenCenter.y + slope * (edgePoint.x - screenCenter.x);
            }
            else
            {
                edgePoint.y = dir.y > 0 ? screenHeight - edgeBuffer : edgeBuffer;
                edgePoint.x = screenCenter.x + (edgePoint.y - screenCenter.y) / slope;
            }

            clampedScreenPos = new Vector2(
                Mathf.Clamp(edgePoint.x, edgeBuffer, screenWidth - edgeBuffer),
                Mathf.Clamp(edgePoint.y, edgeBuffer, screenHeight - edgeBuffer)
            );
        }

        markerUI.position = clampedScreenPos;
        markerUI.gameObject.SetActive(true);

        float scale = Mathf.Clamp(1 / (distanceToTarget * distanceScaleFactor), minSize, maxSize);
        markerUI.localScale = Vector3.one * scale;

        if (!isOnScreen)
        {
            Vector2 directionToTarget = clampedScreenPos - screenCenter;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            markerUI.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            markerUI.rotation = Quaternion.identity;
        }

        if (distanceTMP != null)
        {
            distanceTMP.text = $"{Mathf.RoundToInt(distanceToTarget)}m";
        }
    }
}
