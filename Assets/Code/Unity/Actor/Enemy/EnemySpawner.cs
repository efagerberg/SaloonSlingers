using System.Collections.Generic;

using UnityEngine;

namespace SaloonSlingers.Unity.Slingers
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
        private ActorPool pool;
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

        public GameObject Spawn() => pool.Get(false);

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject go = Spawn();
            go.transform.SetPositionAndRotation(position, rotation);
            return go;
        }

        private void Awake()
        {
            if (pool == null) pool = GetComponent<ActorPool>();
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
            if (pool.CountActive == maxActiveEnemies) return;

            int randomSpawnpointIndex = Random.Range(0, spawnPoints.Count - 1);
            Transform spawnPoint = spawnPoints[randomSpawnpointIndex];
            Vector3 positionNoise = new(Random.Range(minPositionNoise, maxPositionNoise),
                                        Random.Range(minPositionNoise, maxPositionNoise),
                                        0f);
            Vector3 spawnPosition = spawnPoint.position + positionNoise;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(minRotationNoise, maxRotationNoise), 0f);
            GameObject go = Spawn(spawnPosition, rotation);
            go.SetActive(true);
        }
    }
}
