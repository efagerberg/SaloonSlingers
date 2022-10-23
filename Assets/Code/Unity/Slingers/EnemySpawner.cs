using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.Slingers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private int poolSize = 10;
        [SerializeField]
        private List<Transform> spawnPoints;
        [SerializeField]
        private GameObject enemyPrefab;
        [SerializeField]
        private float spawnPerSecond = 4f;
        [SerializeField]
        private int maxActiveEnemies = 3;

        private IObjectPool<Enemy> enemyPool;
        private int activeEnemies = 0;

        public Enemy Spawn() => enemyPool.Get();
        public Enemy Spawn(Vector3 position, Quaternion rotation)
        {
            Enemy e = Spawn();
            e.transform.SetPositionAndRotation(position, rotation);
            activeEnemies += 1;
            return e;
        }
        public void Despawn(Enemy e)
        {
            enemyPool.Release(e);
            activeEnemies -= 1;
        }

        private void Start()
        {
            enemyPool = new ObjectPool<Enemy>(CreateInstance, OnGet, OnRelease, defaultCapacity: poolSize);
            InvokeRepeating(nameof(SpawnEnemy), 1, spawnPerSecond);
        }

        private Enemy CreateInstance()
        {
            GameObject go = Instantiate(enemyPrefab, transform);
            go.SetActive(false);
            return go.GetComponent<Enemy>();
        }

        private void OnGet(Enemy enemy)
        {
            enemy.Attributes = new EnemyAttributes(new Deck().Shuffle(), new Points(1), Handedness.RIGHT);
        }
        private void OnRelease(Enemy enemy)
        {
            enemy.gameObject.SetActive(false);
        }

        private void SpawnEnemy()
        {
            if (activeEnemies >= maxActiveEnemies) return;

            int randomSpawnpointIndex = Random.Range(0, spawnPoints.Count - 1);
            Transform spawnPoint = spawnPoints[randomSpawnpointIndex];
            Enemy enemy = Spawn(spawnPoint.position, Quaternion.identity);
            enemy.gameObject.SetActive(true);
        }
    }
}
