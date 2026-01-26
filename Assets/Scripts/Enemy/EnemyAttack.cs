using UnityEngine;

public sealed class EnemyAttack : MonoBehaviour
{
    private Transform m_target;
    private float m_damage;
    private float m_cooldown;
    private float m_timer;

    private bool m_isInitialized;

    public void Initialize(float damage, float cooldown, Transform target)
    {
        if (m_isInitialized)
            return;

        m_damage = damage;
        m_cooldown = cooldown;
        m_target = target;

        m_isInitialized = true;
    }

    private void Update()
    {
        if (!m_isInitialized)
            return;

        if (m_timer > 0)
            m_timer -= Time.deltaTime;
    }

    public bool TryAttack()
    {
        if (!m_isInitialized || m_timer > 0 || !m_target)
            return false;

        if (m_target.TryGetComponent(out HealthComponent health))
        {
            health.TakeDamage(m_damage);
        }

        m_timer = m_cooldown;
        return true;
    }
}