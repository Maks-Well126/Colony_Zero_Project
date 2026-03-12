using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HintMarker3 : MonoBehaviour
{
    [Header("Target Settings")]
    public List<Transform> targets = new List<Transform>(); // Список целей
    public RectTransform markerPrefab; // Префаб маркера
    private List<RectTransform> markers = new List<RectTransform>(); // Список созданных маркеров
    private List<TMP_Text> markerDistances = new List<TMP_Text>(); // TMP для расстояний

    [Header("Player and Camera")]
    public Transform player;        // Игрок
    public Camera mainCamera;       // Камера
    public Vector3 offset = new Vector3(0, 2, 0); // Смещение маркера в мире

    [Header("Scale Settings")]
    public float maxSize = 1.5f;
    public float minSize = 0.5f;
    public float distanceScaleFactor = 0.05f;

    [Header("Screen Edge Settings")]
    public float edgeBuffer = 50f;

    [Header("Hide Marker Settings")]
    public float hideDistance = 1.5f; // Скрытие маркера при приближении
    public float screenOverlapThreshold = 30f;

    void Start()
    {
        if (markerPrefab == null)
        {
            Debug.LogError("Marker Prefab is not assigned!");
            return;
        }  

        // Скрываем основной markerPrefab, чтобы он не отображался в центре
        markerPrefab.gameObject.SetActive(false);

        // Создаем маркеры для каждой цели
        foreach (Transform t in targets)
        {
            RectTransform newMarker = Instantiate(markerPrefab, markerPrefab.parent);
            newMarker.gameObject.SetActive(true); // включаем только копии
            markers.Add(newMarker);

            TMP_Text tmp = newMarker.GetComponentInChildren<TMP_Text>();
            markerDistances.Add(tmp);
        }
    }


    void Update()
    {
        if (targets.Count == 0 || markers.Count == 0 || mainCamera == null || player == null)
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            UpdateMarkerForTarget(i);
        }
    }

    void UpdateMarkerForTarget(int index)
    {
        Transform target = targets[index];
        RectTransform markerUI = markers[index];
        TMP_Text distanceTMP = markerDistances[index];

        if (target == null || markerUI == null)
            return;

        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        float distanceToTarget = Vector3.Distance(player.position, target.position);

        // Проверка наложения
        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(player.position);
        bool closeOnScreen = Vector2.Distance(playerScreenPos, screenPos) < screenOverlapThreshold;

        if (distanceToTarget < hideDistance || closeOnScreen)
        {
            markerUI.gameObject.SetActive(false);
            return;
        }

        // Проверка, за камерой ли цель
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
            // Исправленный расчёт для врезания в углы
            Vector2 fromCenter = new Vector2(screenPos.x, screenPos.y) - screenCenter;
            Vector2 dir = fromCenter.normalized;

            float halfWidth = (screenWidth / 2f) - edgeBuffer;
            float halfHeight = (screenHeight / 2f) - edgeBuffer;

            float slope = dir.y / dir.x;
            Vector2 edgeHit = screenCenter;

            if (Mathf.Abs(slope) < (halfHeight / halfWidth))
            {
                // Достигаем боковой границы
                edgeHit.x += Mathf.Sign(dir.x) * halfWidth;
                edgeHit.y += Mathf.Sign(dir.x) * halfWidth * slope;
            }
            else
            {
                // Достигаем верхней/нижней границы
                edgeHit.y += Mathf.Sign(dir.y) * halfHeight;
                edgeHit.x += Mathf.Sign(dir.y) * halfHeight / slope;
            }

            clampedScreenPos = new Vector2(
                Mathf.Clamp(edgeHit.x, edgeBuffer, screenWidth - edgeBuffer),
                Mathf.Clamp(edgeHit.y, edgeBuffer, screenHeight - edgeBuffer)
            );
        }

        // Позиция маркера
        markerUI.position = clampedScreenPos;
        markerUI.gameObject.SetActive(true);

        // Масштаб
        float scaleValue = Mathf.Clamp(1 / (distanceToTarget * distanceScaleFactor), minSize, maxSize);
        markerUI.localScale = Vector3.one * scaleValue;

        // Поворот
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

        // Дистанция
        if (distanceTMP != null)
        {
            distanceTMP.text = $"{Mathf.RoundToInt(distanceToTarget)}m";
        }
    }
}
