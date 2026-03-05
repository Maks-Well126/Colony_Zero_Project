using UnityEngine;
using System.Collections;

public class HealingPlant : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private HealthComponent m_playerHealth;

    [Header("Heal")]
    [SerializeField] private float m_healAmount = 20f;
    [SerializeField] private float m_regrowTime = 60f;

    [Header("Audio")]
    [SerializeField] private AudioClip m_hitSound;

    [Header("VFX")]
    [SerializeField] private ParticleSystem m_idleEffect;

    private bool m_isClosed;

    private void Start()
    {
        if (m_idleEffect != null)
            m_idleEffect.Play();
    }

    public void Hit()
    {
        if (m_isClosed)
            return;

        m_isClosed = true;

        if (m_animator != null)
            m_animator.SetTrigger("Close");

        if (m_hitSound != null)
            AudioSource.PlayClipAtPoint(m_hitSound, transform.position);

        if (m_idleEffect != null)
            m_idleEffect.Stop();

        if (m_playerHealth != null)
            m_playerHealth.Heal(m_healAmount);

        StartCoroutine(Regrow());
    }

    private IEnumerator Regrow()
    {
        yield return new WaitForSeconds(m_regrowTime);

        m_isClosed = false;

        if (m_animator != null)
            m_animator.SetTrigger("Open");

        if (m_idleEffect != null)
            m_idleEffect.Play();
    }
}