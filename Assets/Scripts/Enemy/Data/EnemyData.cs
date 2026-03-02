using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Xlab/Data/Enemy")]
public sealed class EnemyData : ScriptableObject
{
    [Header("Prefab")]
    [SerializeField] private Enemy m_enemyPrefab;


    [Header("Stats")]
    [SerializeField][Min(1)] private float m_health = 100f;
    [SerializeField][Min(0)] private float m_moveSpeed = 3f;


    [Header("Attack")]
    [SerializeField][Min(0)] private float m_damage = 10f;
    [SerializeField][Min(0)] private float m_attackCooldown = 1.2f;
    [SerializeField][Min(0)] private float m_attackRange = 2f;

    [Header("Vision")]
    [SerializeField][Min(0)] private float m_detectRange = 10f;

    public float detectRange => m_detectRange;


    public Enemy enemyPrefab => m_enemyPrefab;


    public float health => m_health;
    public float moveSpeed => m_moveSpeed;


    public float damage => m_damage;
    public float attackCooldown => m_attackCooldown;
    public float attackRange => m_attackRange;
}