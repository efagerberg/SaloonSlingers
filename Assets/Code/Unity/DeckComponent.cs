using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class DeckComponent : MonoBehaviour
    {
        private Deck deck;
        [SerializeField]
        private GameObject cardPrefab;
        private int spawnCount = 0;
        private float spawnZOffset;

        void Start()
        {
            deck = new Deck(3).Shuffle();
            spawnZOffset = cardPrefab.GetComponent<BoxCollider>().size.z;
        }

        private void Update()
        {
            while (spawnCount < 15)
            {
                SpawnCard(spawnCount);
                spawnCount += 1;
            }
        }

        public GameObject SpawnCard(int index)
        {
            Card card = deck.RemoveFromTop();
            var spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.forward.z * spawnZOffset * index);
            var go = Instantiate(cardPrefab, spawnPosition, Quaternion.identity, transform);
            go.GetComponent<CardComponent>().SetCard(card);
            return go;
        }
    }
}
