using System;
using UnityEngine;

public sealed class HealthComponent : MonoBehaviour
{
    public event Action Died;

    private float m_currentHealth;

    public void Initialize(float health)
    {
        m_currentHealth = health;
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;

        if (m_currentHealth <= 0f)
        {
            Died?.Invoke();
        }
    }
}