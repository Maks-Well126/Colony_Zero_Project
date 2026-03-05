using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAmbientAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_sounds;

    [SerializeField] private float m_triggerDistance = 8f;

    [SerializeField] [Range(0f,1f)]
    private float m_volume = 0.3f;

    private Transform m_player;
    private AudioSource m_audioSource;

    private bool m_hasPlayed;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.spatialBlend = 1f;
        m_audioSource.volume = m_volume;
    }

    public void Initialize(Transform player)
    {
        m_player = player;
    }

    private void Update()
    {
        if (!m_player)
            return;

        float distance = Vector3.Distance(transform.position, m_player.position);

        if (distance <= m_triggerDistance)
        {
            if (!m_hasPlayed)
            {
                PlaySound();
                m_hasPlayed = true;
            }
        }
        else
        {
            m_hasPlayed = false;
        }
    }

    private void PlaySound()
    {
        if (m_sounds.Length == 0)
            return;

        AudioClip clip = m_sounds[Random.Range(0, m_sounds.Length)];

        m_audioSource.pitch = Random.Range(0.9f, 1.1f);
        m_audioSource.PlayOneShot(clip);
    }
}