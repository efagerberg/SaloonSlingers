using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;
        [SerializeField]
        private int poolSize = 10;
        [SerializeField]
        private GameObject deckPosition;
        [SerializeField]
        private List<Transform> spawnPoints;
        [SerializeField]
        private GameObject enemyPrefab;

        private Deck deck;
        private Stack<GameObject> deckCards;
        private IObjectPool<Enemy> enemyPool;

        public Enemy Spawn() => enemyPool.Get();
        public Enemy Spawn(Vector3 position, Quaternion rotation)
        {
            var e = Spawn();
            e.transform.SetPositionAndRotation(position, rotation);
            return e;
        }
        public void Despawn(Enemy e) => enemyPool.Release(e);

        private void Start()
        {
            deck = new Deck(numberOfCards).Shuffle();
            deckCards = new Stack<GameObject>();
            enemyPool = new ObjectPool<Enemy>(CreateInstance, GetFromPool, defaultCapacity: poolSize);
            SpawnDeck();
            InvokeRepeating(nameof(SpawnEnemy), 3f, 8f);
            deck.OnDeckEmpty += (_, __) => CancelInvoke(nameof(SpawnEnemy));
            deck.OnDeckRefilled += (_, __) => InvokeRepeating(nameof(SpawnEnemy), 3f, 8f);
        }

        private void SpawnDeck()
        {
            float yOffset = cardPrefab.GetComponent<BoxCollider>().size.z * cardPrefab.transform.localScale.z;
            for (int i = 0; i < deck.Count; i++)
            {
                var instantiatePosition = new Vector3(
                    deckPosition.transform.position.x,
                    deckPosition.transform.position.y + yOffset * i,
                    deckPosition.transform.position.z
                );
                var go = Instantiate(cardPrefab, instantiatePosition, deckPosition.transform.rotation, deckPosition.transform);
                deckCards.Push(go);
            }
        }

        private void SpawnEnemy()
        {
            var cardGO = deckCards.Pop();
            Destroy(cardGO);
            var randomSpawnpointIndex = UnityEngine.Random.Range(0, spawnPoints.Count - 1);
            var spawnPoint = spawnPoints[randomSpawnpointIndex];
            Spawn(
                new Vector3(spawnPoint.position.x, 1, spawnPoint.position.z),
                Quaternion.identity
            );
        }

        private Enemy CreateInstance()
        {
            var go = Instantiate(enemyPrefab, transform);
            go.SetActive(false);
            return go.GetComponent<Enemy>();
        }

        private void GetFromPool(Enemy enemy)
        {
            Card card = deck.RemoveFromTop();
            enemy.SetCard(card);
            enemy.gameObject.SetActive(true);
        }
    }
}
