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

        void Start()
        {
            deck = new Deck(3).Shuffle();
        }

        private void Update()
        {
            while (spawnCount < 52)
            {
                SpawnCard(spawnCount);
                spawnCount += 1;
            }
        }

        public GameObject SpawnCard(int index)
        {
            Card card = deck.RemoveFromTop();
            var go = Instantiate(cardPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z * 0.02f * index), Quaternion.identity, transform);
            go.GetComponent<CardComponent>().SetCard(card);
            return go;
        }
    }
}
