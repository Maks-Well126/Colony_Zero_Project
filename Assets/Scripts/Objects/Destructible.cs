using UnityEngine;

public sealed class Destructible : MonoBehaviour
{
    [SerializeField] private DestructibleConfig m_config;
    [SerializeField] private ProgressBar m_progressBar;
    [SerializeField] private SoundController m_sound;

    private float m_timer;
    private bool m_isDestroying;

    public void StartDestroy()
    {
        if (m_isDestroying) return;

        m_isDestroying = true;
        m_timer = 0f;

        m_progressBar?.Show(transform);
        m_sound?.PlayLoop(m_config.LoopSound);
    }

    public void UpdateDestroy(float deltaTime)
    {
        if (!m_isDestroying) return;

        m_timer += deltaTime;
        UpdateProgress();

        if (IsComplete) CompleteDestroy();
    }

    public void CancelDestroy()
    {
        if (!m_isDestroying) return;

        ResetState();
        m_progressBar?.Hide();
        m_sound?.Stop();
    }

    private void UpdateProgress()
    {
        float progress = m_timer / m_config.DestroyTime;
        m_progressBar?.SetProgress(progress);
    }

    private void CompleteDestroy()
    {
        m_sound?.PlayOneShot(m_config.CompleteSound);
        m_progressBar?.Hide();
        Destroy(gameObject);
    }

    private void ResetState()
    {
        m_isDestroying = false;
        m_timer = 0f;
    }

    private bool IsComplete => m_timer >= m_config.DestroyTime;
    public bool IsDestroying => m_isDestroying;
    public float DestroyProgress => m_isDestroying ? Mathf.Clamp01(m_timer / m_config.DestroyTime) : 0f;
}