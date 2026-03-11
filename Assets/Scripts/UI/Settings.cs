using Player;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    [Header("Main Settings Panel")]
    [SerializeField] private GameObject m_settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button m_settingsButton;
    [SerializeField] private Button m_closeSettingsButton;

    [Header("Camera controller")]
    [SerializeField] private PlayerCameraController m_cameraController;

    [Header("Sliders")]
    [SerializeField] private Slider m_sliderVolume;
    [SerializeField] private Slider m_sliderSensitivity;

    [SerializeField] private AudioMixer m_mixer;
    private void Awake()
    {
        m_settingsPanel.gameObject.SetActive(false);

        ApplyInitialSettings();
    }
    private void ApplyInitialSettings()
    {
        SetSensitivity(m_sliderSensitivity.value);

        SetVolume(m_sliderVolume.value);
    }

    private void OnEnable()
    {
        m_settingsButton.onClick.AddListener(OpenSettings);
        m_closeSettingsButton.onClick.AddListener(CloseSettings);
        m_sliderSensitivity.onValueChanged.AddListener(SetSensitivity);
        m_sliderVolume.onValueChanged.AddListener(SetVolume);
    }
    private void OnDisable()
    {
        m_settingsButton.onClick.RemoveListener(OpenSettings);
        m_closeSettingsButton.onClick.RemoveListener(CloseSettings);
        m_sliderSensitivity.onValueChanged.RemoveListener(SetSensitivity);
        m_sliderVolume.onValueChanged.RemoveListener(SetVolume);
    }
    private void SetSensitivity(float value)
    {
        m_cameraController.Sensitivity = m_sliderSensitivity.value / 2;
    }

    private void SetVolume(float value)
    {
        m_mixer.SetFloat("MyExposedParam", value);
    }

    private void OpenSettings()
    {
        m_settingsPanel.gameObject.SetActive(true);
    }
    private void CloseSettings()
    {
        m_settingsPanel.gameObject.SetActive(false);
    }
}
