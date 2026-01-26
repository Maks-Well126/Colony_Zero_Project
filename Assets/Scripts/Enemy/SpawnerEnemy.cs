using UnityEngine;
using Random = UnityEngine.Random;

public sealed class SpawnerEnemy : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private EnemyData[] m_enemies;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] m_spawnPoints;

    [Header("Player")]
    [SerializeField] private Transform m_playerTransform;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        foreach (var point in m_spawnPoints)
        {
            var data = GetEnemyData();

            var enemyInstance = Instantiate(
                data.enemyPrefab,
                point.position,
                point.rotation
            );

            enemyInstance.Initialize(data, m_playerTransform);
            enemyInstance.Died += OnEnemyDied;
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.Died -= OnEnemyDied;
        Destroy(enemy.gameObject);
    }

    private EnemyData GetEnemyData() =>
        m_enemies[Random.Range(0, m_enemies.Length)];
}