using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    private AudioSource m_source;

    private void Awake() => SetupAudioSource();

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null) return;
        m_source.clip = clip;
        m_source.Play();
    }

    public void PlayOneShot(AudioClip clip) => m_source?.PlayOneShot(clip);
    public void Stop() => m_source?.Stop();

    private void SetupAudioSource()
    {
        m_source = GetComponent<AudioSource>();
        m_source.loop = true;
        m_source.spatialBlend = 1f;
    }
}