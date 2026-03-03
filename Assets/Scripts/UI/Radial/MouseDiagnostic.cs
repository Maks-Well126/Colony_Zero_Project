using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseDiagnostic : MonoBehaviour
{
    [SerializeField] private Text m_debugText;
    [SerializeField] private bool m_useNewInputSystem = true;

    private string m_log = "";
    private Mouse m_mouse;

    void Start()
    {
        m_mouse = Mouse.current;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        // Очищаем лог каждый кадр для наглядности
        m_log = "=== ДИАГНОСТИКА МЫШИ ===\n";

        // Проверка старой системы
        m_log += $"\n[СТАРАЯ INPUT SYSTEM]";
        m_log += $"\nGetMouseButtonDown(1): {Input.GetMouseButtonDown(1)}";
        m_log += $"\nGetMouseButton(1): {Input.GetMouseButton(1)}";
        m_log += $"\nGetMouseButtonUp(1): {Input.GetMouseButtonUp(1)}";
        m_log += $"\nMouse Present: {Input.mousePresent}";

        // Проверка новой системы
        m_log += $"\n\n[НОВАЯ INPUT SYSTEM]";
        if (m_mouse != null)
        {
            m_log += $"\nMouse.current: OK";
            m_log += $"\nrightButton.wasPressedThisFrame: {m_mouse.rightButton.wasPressedThisFrame}";
            m_log += $"\nrightButton.isPressed: {m_mouse.rightButton.isPressed}";
            m_log += $"\nrightButton.wasReleasedThisFrame: {m_mouse.rightButton.wasReleasedThisFrame}";
        }
        else
        {
            m_log += $"\nMouse.current = NULL!";
        }

        // Проверка позиции мыши
        m_log += $"\n\n[ПОЗИЦИЯ]";
        m_log += $"\nMouse Position: {Input.mousePosition}";
        m_log += $"\nScreen Center: {Screen.width / 2}, {Screen.height / 2}";

        // Проверка всех кнопок мыши
        m_log += $"\n\n[ВСЕ КНОПКИ]";
        for (int i = 0; i < 3; i++)
        {
            m_log += $"\nButton {i} Down: {Input.GetMouseButtonDown(i)}";
        }

        // Отображаем лог
        if (m_debugText != null)
        {
            m_debugText.text = m_log;
        }

        // Принудительный тест
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("🔥🔥🔥 ПКМ НАЖАТА (старая система) 🔥🔥🔥");
        }

        if (m_mouse != null && m_mouse.rightButton.wasPressedThisFrame)
        {
            Debug.Log("🔥🔥🔥 ПКМ НАЖАТА (новая система) 🔥🔥🔥");
        }
    }

    void OnGUI()
    {
        // Альтернативное отображение если нет UI Text
        GUI.Box(new Rect(10, 10, 400, 300), m_log);
    }
}