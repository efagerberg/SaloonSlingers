using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> spawnPoints;
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

        private bool currentlyVisible = false;
        private SaloonManager saloonManager;
        private IDictionary<string, ActorPool> pools = new Dictionary<string, ActorPool>();

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
            saloonManager.Saloon.EnemyInventory.RecordDeath(go.name);

            var actor = go.GetComponent<IActor>();
            actor.Death -= RecordDeath;
        }

        private void Start()
        {
            saloonManager = GameObject.FindGameObjectWithTag("SaloonManager").GetComponent<SaloonManager>();
            foreach (string enemyStr in saloonManager.Saloon.EnemyInventory.Manifest.Keys)
            {
                pools[enemyStr] = gameObject.AddComponent<ActorPool>();
                var prefab = Resources.Load<GameObject>($"prefabs/{enemyStr}");
                pools[enemyStr].Prefab = prefab;
            }
            InvokeRepeating(nameof(SpawnEnemy), 1, spawnPerSecond);
            currentlyVisible = true;
        }

        private void Update()
        {
            if (currentlyVisible != visibleSpawnPointsOnRun)
            {
                foreach (var t in spawnPoints)
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
            var enemyStr = saloonManager.Saloon.EnemyInventory.GetRandomEnemy();
            if (enemyStr == null || pools.Values.Sum(p => p.CountActive) == maxActiveEnemies || saloonManager.Saloon.EnemyInventory.Completed) return;

            int randomSpawnpointIndex = Random.Range(0, spawnPoints.Count - 1);
            Transform spawnPoint = spawnPoints[randomSpawnpointIndex];
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
