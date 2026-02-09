using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public sealed class SpawnerEnemy : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private EnemyData[] m_enemies;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] m_spawnPoints;

    [Header("Player")]
    [SerializeField] private Transform m_playerTransform;

    [Header("Respawn Settings")]
    [SerializeField] private float m_respawnDelay = 5f;

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (var point in m_spawnPoints)
        {
            SpawnEnemy(point);
        }
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        var data = GetEnemyData();

        var enemyInstance = Instantiate(
            data.enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        enemyInstance.Initialize(data, m_playerTransform);

        enemyInstance.Died += enemy => OnEnemyDied(enemy, spawnPoint);
    }

    private void OnEnemyDied(Enemy enemy, Transform spawnPoint)
    {
        enemy.Died -= e => OnEnemyDied(e, spawnPoint);

        Destroy(enemy.gameObject, 4f);

        StartCoroutine(RespawnAfterDelay(spawnPoint));
    }

    private IEnumerator RespawnAfterDelay(Transform spawnPoint)
    {
        yield return new WaitForSeconds(m_respawnDelay);
        SpawnEnemy(spawnPoint);
    }


    private EnemyData GetEnemyData()
    {
        return m_enemies[Random.Range(0, m_enemies.Length)];
    }
}
