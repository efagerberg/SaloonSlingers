using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class DeckComponent : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int cardSpawnLimit = 52;

        private int spawnCount = 0;
        private float spawnZOffset;
        private Deck deck;

        void Start()
        {
            deck = new Deck(3).Shuffle();
            spawnZOffset = cardPrefab.GetComponent<BoxCollider>().size.z;
        }

        private void Update()
        {
            while (spawnCount < cardSpawnLimit)
            {
                SpawnCard(spawnCount);
                spawnCount += 1;
            }
        }

        public GameObject SpawnCard(int index)
        {
            Card card = deck.RemoveFromTop();
            var spawnPosition = new Vector3(transform.position.x, transform.position.y, spawnZOffset * index);
            var go = Instantiate(cardPrefab, spawnPosition, Quaternion.identity, transform);
            go.GetComponent<CardComponent>().SetCard(card);
            return go;
        }
    }
}
