using System;
using UnityEngine;

public sealed class HealthComponent : MonoBehaviour
{
    public event Action<float, float> HealthChanged;
    public event Action Damaged;
    public event Action Died;

    private float m_currentHealth;
    private float m_maxHealth;
    private bool m_isDead;

    public float CurrentHealth => m_currentHealth;
    public float MaxHealth => m_maxHealth;

    public void Initialize(float maxHealth)
    {
        m_maxHealth = maxHealth;
        m_currentHealth = maxHealth;
        m_isDead = false;

        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (m_isDead)
            return;

        m_currentHealth -= damage;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0f, m_maxHealth);

        //if(m_currentHealth <= 0)
        //{
        //    m_isDead = true;
        //    Died?.Invoke();
        //}
        //else
        //{
        //    Damaged?.Invoke();
        //    HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
        //}

        Damaged?.Invoke();
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);

        if (m_currentHealth <= 0f)
        {
            m_isDead = true;
            Died?.Invoke();
        }
    }
    private void Update()
    {
       // Debug.Log(m_currentHealth);
    }
    public void HealToFull()
    {
        if (m_isDead)
            return;

        m_currentHealth = m_maxHealth;
        HealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }
}