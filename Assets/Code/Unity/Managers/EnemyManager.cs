using System;
using System.Collections.Generic;

using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class EnemyManager : MonoBehaviour, ISpawner<GameObject>
    {
        public UnityEvent<EnemyManager, Actor.Actor> OnEnemyKilled;

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
            var actor = go.GetComponent<Actor.Actor>();
            actor.OnKilled.AddListener(EnemyKilledHandler);
            return go;
        }

        public IList<Transform> EnemyWayPoints => enemySpawner.SpawnPoints;

        private void Start()
        {
            InvokeRepeating(nameof(Spawn), 1, spawnPerSecond);
            GameManager.Instance.Saloon.EnemyInventory.Emptied += OnInventoryEmptied;
        }

        private void EnemyKilledHandler(Actor.Actor sender)
        {
            enemiesSpawned--;
            OnEnemyKilled?.Invoke(this, sender);
            var actor = sender.GetComponent<Actor.Actor>();
            actor.OnKilled.RemoveListener(EnemyKilledHandler);
        }

        private void OnInventoryEmptied(object sender, EventArgs e)
        {
            CancelInvoke(nameof(Spawn));
        }
    }
}

