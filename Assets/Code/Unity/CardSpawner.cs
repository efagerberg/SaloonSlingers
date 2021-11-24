using System.Collections.Generic;

using GambitSimulator.Core;

using UnityEngine;
using UnityEngine.Pool;


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

        public IObjectPool<CardInteractable> Pool;

        private void Start()
        {
            Pool = new ObjectPool<CardInteractable>(CreateInstance, GetFromPool, defaultCapacity: poolSize);
            CardInteractable.OnCardDeactivated += (sender, _) => Pool.Release(sender);
        }

        private CardInteractable CreateInstance()
        {
            var go = Instantiate(cardPrefab, transform);
            var cardInteractable = go.GetComponent<CardInteractable>();
            go.SetActive(false);
            return cardInteractable;
        }

        private void GetFromPool(CardInteractable cardInteractable)
        {
            Card card = playerAttributes.Deck.RemoveFromTop();
            cardInteractable.gameObject.SetActive(true);
            cardInteractable.SetCard(card);
        }
    }
}
