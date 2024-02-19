using System.Collections.Generic;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemySpawner : MonoBehaviour, ISpawner<GameObject>
    {
        public List<Transform> SpawnPoints;

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

        public GameObject Spawn()
        {
            var enemyStr = gameManager.Saloon.EnemyInventory.GetRandomEnemy();
            if (enemyStr == null)
            {
                Debug.LogError("No enemies left to spawn despite inventory not being marked complete");
                return null;
            }

            int randomSpawnpointIndex = Random.Range(0, SpawnPoints.Count - 1);
            Transform spawnPoint = SpawnPoints[randomSpawnpointIndex];
            Vector3 positionNoise = new(Random.Range(minPositionNoise, maxPositionNoise),
                                        Random.Range(minPositionNoise, maxPositionNoise),
                                        0f);
            Vector3 spawnPosition = spawnPoint.position + positionNoise;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(minRotationNoise, maxRotationNoise), 0f);
            GameObject go = SetupSpawned(spawnPosition, rotation, enemyStr);
            go.SetActive(true);
            return go;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            foreach (string enemyStr in gameManager.Saloon.EnemyInventory.Manifest.Keys)
            {
                var prefab = Resources.Load<GameObject>($"prefabs/{enemyStr}");
                pools[enemyStr] = new ActorPool(poolSize, prefab, transform);
            }
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

        private GameObject SetupSpawned(Vector3 position, Quaternion rotation, string enemyStr)
        {
            GameObject go = pools[enemyStr].Get(false);
            go.transform.SetPositionAndRotation(position, rotation);
            go.name = enemyStr;
            return go;
        }
    }
}
