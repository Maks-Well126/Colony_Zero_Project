using UnityEngine;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    [Header("Main Settings Panel")]
    [SerializeField] private Button m_settingsButton;

    private void Start()
    {
        m_settingsButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_settingsButton.onClick.AddListener(OpenSettings);
    }
    private void OnDisable()
    {
        m_settingsButton.onClick.RemoveListener(OpenSettings);
    }

    private void OpenSettings()
    {
        m_settingsButton.gameObject.SetActive(true);
    }
}
