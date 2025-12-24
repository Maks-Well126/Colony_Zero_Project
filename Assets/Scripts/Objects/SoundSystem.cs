using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip m_collectingSound;
    [SerializeField] private AudioClip m_completeSound;
    [SerializeField] private float m_volume = 0.7f;
    [SerializeField] private bool m_loopCollectingSound = true;

    private AudioSource m_audioSource;
    private bool m_isPlayingCollectingSound = false;

    public void Initialize(AudioSource source)
    {
        m_audioSource = source;
        ConfigureAudioSource();
    }

    public void StartCollecting()
    {
        PlayCollectingSound(true);
    }

    public void StopCollecting()
    {
        PlayCollectingSound(false);
    }

    public void PlayCompleteSound()
    {
        StopCollecting();
        PlaySoundOnce(m_completeSound);
    }

    public void StopAllSounds()
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
        }
        m_isPlayingCollectingSound = false;
    }

    private void ConfigureAudioSource()
    {
        if (m_audioSource == null) return;

        m_audioSource.spatialBlend = 1f;
        m_audioSource.maxDistance = 10f;
        m_audioSource.volume = m_volume;
    }

    private void PlayCollectingSound(bool shouldPlay)
    {
        if (!CanPlayCollectingSound()) return;

        if (shouldPlay && !m_isPlayingCollectingSound)
        {
            StartCollectingSound();
        }
        else if (!shouldPlay && m_isPlayingCollectingSound)
        {
            StopCollectingSound();
        }
    }

    private bool CanPlayCollectingSound()
    {
        return m_collectingSound != null && m_audioSource != null;
    }

    private void StartCollectingSound()
    {
        m_audioSource.clip = m_collectingSound;
        m_audioSource.loop = m_loopCollectingSound;
        m_audioSource.Play();
        m_isPlayingCollectingSound = true;
    }

    private void StopCollectingSound()
    {
        m_audioSource.Stop();
        m_isPlayingCollectingSound = false;
    }

    private void PlaySoundOnce(AudioClip clip)
    {
        if (clip == null || m_audioSource == null) return;
        m_audioSource.PlayOneShot(clip, m_volume);
    }
}