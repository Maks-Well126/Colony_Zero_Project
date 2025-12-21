using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Xlab/Data/EnemyData")]
public sealed class EnemyData : ScriptableObject
{
    [Header("Stats")]
    [SerializeField][Min(1)] private float m_maxHealth = 100f;
    [SerializeField][Min(0)] private float m_moveSpeed = 3f;
    [SerializeField][Min(0)] private float m_damage = 10f;

    [Header("Combat")]
    [SerializeField][Min(0)] private float m_attackRange = 2f;
    [SerializeField][Min(0)] private float m_attackCooldown = 1.2f;

    [Header("Vision")]
    [SerializeField][Min(0)] private float m_viewDistance = 10f;
    [SerializeField][Range(0, 180)] private float m_viewAngle = 120f;

    [Header("Visual")]
    [SerializeField] private GameObject m_modelPrefab;

    public float maxHealth => m_maxHealth;
    public float moveSpeed => m_moveSpeed;
    public float damage => m_damage;

    public float attackRange => m_attackRange;
    public float attackCooldown => m_attackCooldown;

    public float viewDistance => m_viewDistance;
    public float viewAngle => m_viewAngle;

    public GameObject modelPrefab => m_modelPrefab;
}
