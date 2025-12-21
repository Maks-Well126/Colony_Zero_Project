using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData m_data;

    private float m_currentHealth;

    private Transform m_player;

    private void Awake()
    {
        m_currentHealth = m_data.maxHealth;

        m_player = GameObject.FindWithTag("Player").transform;

        if (m_data.modelPrefab != null)
        {
            Instantiate(
                m_data.modelPrefab,
                transform.position,
                transform.rotation,
                transform
            );
        }
    }

    private void Update()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (m_player == null) return;

        Vector3 direction = m_player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude > m_data.attackRange)
        {
            transform.position +=
                direction.normalized *
                m_data.moveSpeed *
                Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;

        if (m_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
