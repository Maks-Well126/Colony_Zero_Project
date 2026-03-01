using UnityEngine;
using System;
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

    private bool m_isActive;

    public void Spawn()
    {
        m_isActive = true;

        foreach (var point in m_spawnPoints)
        {
            SpawnEnemy(point);
        }
    }

    // public void Stop()
    // {
    //     m_isActive = false;
    //     StopAllCoroutines();
    // }

    public void ClearAll()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        if (!m_isActive)
            return;

        float checkRadius = 1f;
        Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, checkRadius);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<Enemy>(out _))
            {
                return;
            }
        }

        var data = GetEnemyData();

        var enemyInstance = Instantiate(
            data.enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        enemyInstance.Initialize(data, m_playerTransform);

        Action<Enemy> handler = null;
        handler = enemy =>
        {
            enemy.Died -= handler;
            OnEnemyDied(enemy, spawnPoint);
        };

        enemyInstance.Died += handler;
    }

    private void OnEnemyDied(Enemy enemy, Transform spawnPoint)
    {
        Destroy(enemy.gameObject, 4f);

        if (!m_isActive)
            return;

        StartCoroutine(RespawnAfterDelay(spawnPoint));
    }

    private IEnumerator RespawnAfterDelay(Transform spawnPoint)
    {
        yield return new WaitForSeconds(m_respawnDelay);

        if (!m_isActive)
            yield break;

        SpawnEnemy(spawnPoint);
    }

    private EnemyData GetEnemyData()
    {
        return m_enemies[Random.Range(0, m_enemies.Length)];
    }
}