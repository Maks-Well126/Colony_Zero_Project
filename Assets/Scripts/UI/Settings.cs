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

    private const string SENSITIVITY_KEY = "Sensitivity";
    private const string VOLUME_KEY = "Volume";
    private const float DEFAULT_VOLUME = -20f;
    private const float DEFAULT_SENSITIVITY = 0.1f;

    private static AudioMixer s_mixer;
    private static PlayerCameraController s_cameraController;

    public static void InitializeAudio(AudioMixer mixer, PlayerCameraController cameraController)
    {
        s_mixer = mixer;
        s_cameraController = cameraController;

        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        float savedSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, DEFAULT_SENSITIVITY);

        s_mixer.SetFloat("MyExposedParam", savedVolume);
        s_cameraController.Sensitivity = savedSensitivity / 2;
    }

    private void Awake()
    {
        m_settingsPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        LoadSettings();
        ApplyInitialSettings();
    }
    private void ApplyInitialSettings()
    {
        SetSensitivity(m_sliderSensitivity.value);

        SetVolume(m_sliderVolume.value);
    }

    private void LoadSettings()
    {
        m_sliderSensitivity.value = PlayerPrefs.GetFloat(SENSITIVITY_KEY, DEFAULT_SENSITIVITY);
        m_sliderVolume.value = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
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
        m_cameraController.Sensitivity = value / 2;
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
    }

    private void SetVolume(float value)
    {
        m_mixer.SetFloat("MyExposedParam", value);
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
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