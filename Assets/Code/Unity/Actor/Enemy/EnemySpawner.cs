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
        }

        private void SpawnEnemy()
        {
            if (pool.CountActive == maxActiveEnemies) return;

            int randomSpawnpointIndex = Random.Range(0, spawnPoints.Count - 1);
            Transform spawnPoint = spawnPoints[randomSpawnpointIndex];
            GameObject go = Spawn(spawnPoint.position, Quaternion.identity);
            go.SetActive(true);
        }
    }
}
