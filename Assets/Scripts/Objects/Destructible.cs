using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Destructible : MonoBehaviour
{
    // [Header("Settings")]
    // [SerializeField] private float m_destroyTime = 2f;

   // [Header("Audio")]
   // [SerializeField] private AudioClip m_processSound;
    //[SerializeField] private AudioClip m_completeSound;
    [SerializeField] private DestructibleConfig m_config; 

    private float m_timer;
    private bool m_isDestroying;
    

    private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
        m_audioSource.playOnAwake = false;
        m_audioSource.loop = true;
    }

    public void StartDestroy()
    {
        if (m_isDestroying) return;

        m_isDestroying = true;
        m_timer = 0f;

        if (m_config.LoopSound != null)
        {
            m_audioSource.clip = m_config.LoopSound;
            m_audioSource.loop = true;
            m_audioSource.Play();
        }
    }

    public void UpdateDestroy(float deltaTime)
    {
        if (!m_isDestroying) return;

        m_timer += deltaTime;

        if (m_timer >= m_config.DestroyTime)
        {
            CompleteDestroy();
        }
    }

    public void CancelDestroy()
    {
        if (!m_isDestroying) return;

        m_isDestroying = false;
        m_timer = 0f;

        m_audioSource.Stop();
    }

    private void CompleteDestroy()
    {
        m_audioSource.Stop();

        if (m_config.CompleteSound != null)
        {
            AudioSource.PlayClipAtPoint(m_config.CompleteSound, transform.position);
        }

        Destroy(gameObject);
    }
}