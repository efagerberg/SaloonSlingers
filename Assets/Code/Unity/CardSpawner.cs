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
        private bool hasRegisteredDeckEvents = false;

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
            pool = new ObjectPool<CardInteractable>(CreateInstance, GetFromPool, defaultCapacity: poolSize);
        }

        private void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            player = playerGO.GetComponent<Player>();
            RegisterDeckEvents();
            hasCardsLeft = player.Attributes.Deck.Count > 0;
        }

        private void OnEnable()
        {
            CardInteractable.OnCardDeactivated += CardDeactivatedHandler;
            RegisterDeckEvents();
        }

        private void OnDisable()
        {
            CardInteractable.OnCardDeactivated += CardDeactivatedHandler;
            UnregisterDeckEvents();
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

        private void CardDeactivatedHandler(CardInteractable sender, EventArgs _) => Despawn(sender);
        private void DeckEmptyHandler(Deck sender, EventArgs _) => hasCardsLeft = false;
        private void DeckRefilledHandler(Deck sender, EventArgs _) => hasCardsLeft = true;
        private void RegisterDeckEvents()
        {
            if (hasRegisteredDeckEvents || player == null) return;
            player.Attributes.Deck.OnDeckEmpty += DeckEmptyHandler;
            player.Attributes.Deck.OnDeckRefilled += DeckRefilledHandler;
            hasRegisteredDeckEvents = true;
        }
        private void UnregisterDeckEvents()
        {
            if (!hasRegisteredDeckEvents || player == null) return;
            player.Attributes.Deck.OnDeckEmpty -= DeckEmptyHandler;
            player.Attributes.Deck.OnDeckRefilled -= DeckRefilledHandler;
            hasRegisteredDeckEvents = false;
        }
    }
}
