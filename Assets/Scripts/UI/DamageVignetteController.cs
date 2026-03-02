using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageVignetteController : MonoBehaviour
{
    [SerializeField] private Volume m_volume;
    [SerializeField] private float m_smoothSpeed = 5f;
    [SerializeField] private float m_maxIntensity = 0.45f;

    private Vignette m_vignette;
    private float m_targetIntensity;

    private void Awake()
    {
        m_volume.profile.TryGet(out m_vignette);
        m_vignette.intensity.value = 0f;
    }

    private void Update()
    {
        m_vignette.intensity.value = Mathf.Lerp(
            m_vignette.intensity.value,
            m_targetIntensity,
            Time.deltaTime * m_smoothSpeed
        );
    }

    public void OnHealthChanged(float current, float max)
    {
        float healthPercent = current / max;

        m_targetIntensity = Mathf.Lerp(
            m_maxIntensity,
            0f,
            healthPercent
        );
    }

    public void FlashDamage()
    {
        m_vignette.intensity.value = Mathf.Min(
            m_vignette.intensity.value + 0.1f,
            m_maxIntensity
        );
    }

    public void OnDeath()
    {
        m_targetIntensity = m_maxIntensity;
    }
}