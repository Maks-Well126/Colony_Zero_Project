using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    private AudioSource m_source;

    private void Awake()
    {
        m_source = GetComponent<AudioSource>();
        m_source.loop = true;
        m_source.spatialBlend = 1f;
    }

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null) return;

        m_source.clip = clip;
        m_source.Play();
    }

    public void Stop()
    {
        if (m_source.isPlaying)
            m_source.Stop();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;

        m_source.PlayOneShot(clip);
    }
}
