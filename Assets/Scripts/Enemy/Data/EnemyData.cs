using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData",menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float damage = 10f;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;

    [Header("Vision")]
    public float viewDistance = 10f;
    public float viewAngle = 120f;

    [Header("Visual")]
    public GameObject modelPrefab;
}
