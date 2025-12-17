using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Points")]
    [Tooltip("Enemy prefab from project assets (not in scene hierarchy).")]
    public GameObject enemyPrefab;

    [Tooltip("If empty, spawns at this spawner's transform.")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [Min(1)] public int maxAlive = 5;            // cap simultaneous enemies
    [Min(0f)] public float spawnInterval = 2f;    // seconds between spawns
    public bool spawnOnStart = true;

    // Runtime tracking
    private int aliveCount = 0;
    private Coroutine spawnRoutine;

    void Start()
    {
        if (spawnOnStart) StartSpawning();
    }

    [ContextMenu("Start Spawning")]
    public void StartSpawning()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Missing enemyPrefab.", this);
            return;
        }

        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Only spawn if under maxAlive
            if (aliveCount < maxAlive)
            {
                SpawnOne();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        Transform point = ChooseSpawnPoint();
        GameObject go = Instantiate(enemyPrefab, point.position, point.rotation);

        // Wire health death event so we keep accurate alive counts
        var hc = go.GetComponentInChildren<HealthComponent>();
        if (hc != null)
        {
            aliveCount++;

            void OnDiedHandler()
            {
                aliveCount = Mathf.Max(0, aliveCount - 1);
                hc.OnDied -= OnDiedHandler; // clean up
            }
            hc.OnDied += OnDiedHandler;
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] Spawned enemy has no HealthComponent; alive count won't track.", go);
        }
    }

    private Transform ChooseSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return transform;
        int idx = Random.Range(0, spawnPoints.Length);
        return spawnPoints[idx] ? spawnPoints[idx] : transform;
    }

    // Optional: expose alive count
    public int AliveCount => aliveCount;
}
