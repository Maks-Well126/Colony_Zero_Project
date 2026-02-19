using System;
using UnityEngine;

public sealed class HealthComponent : MonoBehaviour
{
    public event Action Damaged;
    public event Action Died;


    private float m_currentHealth;
    private bool m_isDead;

    public void Initialize(float health)
    {
        m_currentHealth = health;
        m_isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (m_isDead)
            return;

        m_currentHealth -= damage;
        
        if (m_currentHealth <= 0f)
        {
            m_isDead = true;
            Died?.Invoke();
            return;
        }

        Damaged?.Invoke();
    }
}
