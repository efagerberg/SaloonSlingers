using System;
using System.Collections.Generic;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class EnemyManager : MonoBehaviour, ISpawner<GameObject>
    {
        public EventHandler EnemyKilled;

        [SerializeField]
        private int maxActiveEnemies = 3;
        [SerializeField]
        private float spawnPerSecond = 4f;
        [SerializeField]
        private EnemySpawner enemySpawner;

        private int enemiesSpawned = 0;

        public GameObject Spawn()
        {
            if (enemiesSpawned == maxActiveEnemies) return null;

            var go = enemySpawner.Spawn();
            enemiesSpawned++;
            var actor = go.GetComponent<IActor>();
            actor.Killed += OnEnemyKilled;
            return go;
        }

        public IList<Transform> EnemyWayPoints => enemySpawner.SpawnPoints;

        private void Start()
        {
            InvokeRepeating(nameof(Spawn), 1, spawnPerSecond);
            GameManager.Instance.Saloon.EnemyInventory.Emptied += OnInventoryEmptied;
        }

        private void OnEnemyKilled(object sender, EventArgs args)
        {
            enemiesSpawned--;
            EnemyKilled?.Invoke(sender, args);
        }

        private void OnInventoryEmptied(object sender, EventArgs e)
        {
            CancelInvoke(nameof(Spawn));
        }
    }
}

