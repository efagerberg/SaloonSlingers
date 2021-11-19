using UnityEngine;

using GambitSimulator.Core;
using System.Collections.Generic;

namespace GambitSimulator.Unity
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private PlayerAttributes playerAttributes;
        [SerializeField]
        private int poolSize = 10;

        private Queue<CardInteractable> pool;

        private void Start()
        {
            pool = new Queue<CardInteractable>();
            for (int i = 0; i < poolSize; i++)
            {
                var go = Instantiate(cardPrefab, transform);
                var cardInteractable = go.GetComponent<CardInteractable>();
                go.SetActive(false);
                pool.Enqueue(cardInteractable);
            }
            CardInteractable.OnCardDeactivated += (sender, _) => pool.Enqueue(sender);
        }

        public bool TrySpawnCard(Vector3 spawnPosition, out CardInteractable cardInteractable)
        {
            cardInteractable = null;
            if (pool.Count == 0 || playerAttributes.Deck.Count == 0) return false;

            Card card = playerAttributes.Deck.RemoveFromTop();
            var cardInteractableFromPool = pool.Dequeue();
            cardInteractableFromPool.gameObject.SetActive(true);
            cardInteractableFromPool.transform.position = spawnPosition;
            cardInteractableFromPool.SetCard(card);
            cardInteractable = cardInteractableFromPool;
            return true;
        }
    }
}
