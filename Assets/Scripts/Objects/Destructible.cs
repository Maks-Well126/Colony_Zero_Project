using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour
{
    [Header("Destroy Settings")]
    [SerializeField] private float m_destroyTime = 2f;

    [Header("Sink Settings")]
    [SerializeField] private float m_sinkDistance = 2f;
    [SerializeField] private float m_sinkSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip m_processSound;
    [SerializeField] private AudioClip m_completeSound;

    private float m_timer;
    private bool m_isDestroying;
    private bool m_isSinking;

    private AudioSource m_audioSource;
    private Collider m_collider;

    private void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
        m_audioSource.playOnAwake = false;
        m_audioSource.loop = true;

        m_collider = GetComponent<Collider>();
    }

    public void StartDestroy()
    {
        if (m_isDestroying || m_isSinking) return;

        m_isDestroying = true;
        m_timer = 0f;

        if (m_processSound != null)
        {
            m_audioSource.clip = m_processSound;
            m_audioSource.loop = true;
            m_audioSource.Play();
        }
    }

    public void UpdateDestroy(float deltaTime)
    {
        if (!m_isDestroying || m_isSinking) return;

        m_timer += deltaTime;

        if (m_timer >= m_destroyTime)
        {
            CompleteDestroy();
        }
    }

    public void CancelDestroy()
    {
        if (!m_isDestroying || m_isSinking) return;

        m_isDestroying = false;
        m_timer = 0f;
        m_audioSource.Stop();
    }

    private void CompleteDestroy()
    {
        m_isDestroying = false;
        m_isSinking = true;

        m_audioSource.Stop();

        if (m_completeSound != null)
        {
            AudioSource.PlayClipAtPoint(m_completeSound, transform.position);
        }

        if (m_collider != null)
            m_collider.enabled = false;

        StartCoroutine(SinkAndDestroy());
    }

    private IEnumerator SinkAndDestroy()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.down * m_sinkDistance;

        while (transform.position.y > targetPos.y)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                m_sinkSpeed * Time.deltaTime
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}