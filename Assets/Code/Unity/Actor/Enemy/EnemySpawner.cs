using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemySpawner : MonoBehaviour
    {
        public List<Transform> SpawnPoints;

        [SerializeField]
        private float spawnPerSecond = 4f;
        [SerializeField]
        private int maxActiveEnemies = 3;
        [SerializeField]
        private bool visibleSpawnPointsOnRun = false;
        [SerializeField]
        private float minPositionNoise = -0.25f;
        [SerializeField]
        private float maxPositionNoise = 0.25f;
        [SerializeField]
        private float minRotationNoise = 0;
        [SerializeField]
        private float maxRotationNoise = 200f;
        [SerializeField]
        private int poolSize = 10;

        private bool currentlyVisible = false;
        private GameManager gameManager;
        private readonly IDictionary<string, ActorPool> pools = new Dictionary<string, ActorPool>();

        public GameObject Spawn(string enemyStr) => pools[enemyStr].Get(false);

        public GameObject Spawn(Vector3 position, Quaternion rotation, string enemyStr)
        {
            GameObject go = Spawn(enemyStr);
            go.transform.SetPositionAndRotation(position, rotation);
            go.name = enemyStr;
            var actor = go.GetComponent<IActor>();
            actor.Death += RecordDeath;
            return go;
        }

        private void RecordDeath(object sender, System.EventArgs e)
        {
            var go = (GameObject)sender;
            gameManager.Saloon.EnemyInventory.RecordDeath(go.name);

            var actor = go.GetComponent<IActor>();
            actor.Death -= RecordDeath;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            foreach (string enemyStr in gameManager.Saloon.EnemyInventory.Manifest.Keys)
            {
                var prefab = Resources.Load<GameObject>($"prefabs/{enemyStr}");
                pools[enemyStr] = new ActorPool(poolSize, prefab, transform);
            }
            InvokeRepeating(nameof(SpawnEnemy), 1, spawnPerSecond);
            currentlyVisible = true;
        }

        private void Update()
        {
            if (currentlyVisible != visibleSpawnPointsOnRun)
            {
                foreach (var t in SpawnPoints)
                {
                    for (int i = 0; i < t.childCount; i++)
                    {
                        t.GetChild(i).gameObject.SetActive(visibleSpawnPointsOnRun);
                    }
                }
                currentlyVisible = visibleSpawnPointsOnRun;
            }
        }

        private void SpawnEnemy()
        {
            if (pools.Values.Sum(p => p.CountSpanwed) == maxActiveEnemies || gameManager.Saloon.EnemyInventory.Completed) return;

            var enemyStr = gameManager.Saloon.EnemyInventory.GetRandomEnemy();
            if (enemyStr == null)
            {
                Debug.LogError("No enemies left to spawn despite inventory not being marked complete");
                return;
            }

            int randomSpawnpointIndex = Random.Range(0, SpawnPoints.Count - 1);
            Transform spawnPoint = SpawnPoints[randomSpawnpointIndex];
            Vector3 positionNoise = new(Random.Range(minPositionNoise, maxPositionNoise),
                                        Random.Range(minPositionNoise, maxPositionNoise),
                                        0f);
            Vector3 spawnPosition = spawnPoint.position + positionNoise;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(minRotationNoise, maxRotationNoise), 0f);
            GameObject go = Spawn(spawnPosition, rotation, enemyStr);
            go.SetActive(true);
        }
    }
}
