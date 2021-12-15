using System;

using UnityEngine;
using UnityEngine.Pool;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int poolSize = 10;

        private IObjectPool<CardInteractable> pool;
        private Player player;
        private bool hasCardsLeft;

        public CardInteractable Spawn() => pool.Get();
        public CardInteractable Spawn(Vector3 position, Quaternion rotation)
        {
            var c = Spawn();
            c.transform.SetPositionAndRotation(position, rotation);
            return c;
        }
        public void Despawn(CardInteractable c) => pool.Release(c);
        public bool HasCardsLeft() => hasCardsLeft;

        private void Awake()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            player = playerGO.GetComponent<Player>();
            hasCardsLeft = player.Attributes.Deck.Count > 0;
            pool = new ObjectPool<CardInteractable>(CreateInstance, GetFromPool, defaultCapacity: poolSize);
        }

        private void OnEnable()
        {
            CardInteractable.OnCardDeactivated += cardDeactivatedHandler;
            player.Attributes.Deck.OnDeckEmpty += deckEmptyHandler;
            player.Attributes.Deck.OnDeckRefilled += deckRefilledHandler;
        }

        private void OnDisable()
        {
            CardInteractable.OnCardDeactivated -= cardDeactivatedHandler;
            player.Attributes.Deck.OnDeckEmpty -= deckEmptyHandler;
            player.Attributes.Deck.OnDeckRefilled -= deckRefilledHandler;
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
            Card card = player.Attributes.Deck.RemoveFromTop();
            cardInteractable.gameObject.SetActive(true);
            cardInteractable.SetCard(card);
        }

        private void cardDeactivatedHandler(CardInteractable sender, EventArgs _) => Despawn(sender);
        private void deckEmptyHandler(Deck sender, EventArgs _) => hasCardsLeft = false;
        private void deckRefilledHandler(Deck sender, EventArgs _) => hasCardsLeft = true;
    }
}
