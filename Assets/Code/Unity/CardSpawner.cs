using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int poolSize = 10;

        private IObjectPool<CardInteractable> pool;
        private PlayerAttributes Attributes;
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
            Player player = playerGO.GetComponent<Player>();
            Attributes = (PlayerAttributes)player.Attributes;
            RegisterDeckEvents();
            hasCardsLeft = Attributes.Deck.Count > 0;
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
            Card card = Attributes.Deck.RemoveFromTop();
            cardInteractable.gameObject.SetActive(true);
            cardInteractable.SetCard(card);
            Attributes.Hand.Add(card);
        }

        private void CardDeactivatedHandler(CardInteractable sender, EventArgs _) => Despawn(sender);
        private void DeckEmptyHandler(Deck sender, EventArgs _) => hasCardsLeft = false;
        private void DeckRefilledHandler(Deck sender, EventArgs _) => hasCardsLeft = true;
        private void RegisterDeckEvents()
        {
            if (hasRegisteredDeckEvents || Attributes.Deck == null) return;
            Attributes.Deck.OnDeckEmpty += DeckEmptyHandler;
            Attributes.Deck.OnDeckRefilled += DeckRefilledHandler;
            hasRegisteredDeckEvents = true;
        }
        private void UnregisterDeckEvents()
        {
            if (!hasRegisteredDeckEvents || Attributes.Deck == null) return;
            Attributes.Deck.OnDeckEmpty -= DeckEmptyHandler;
            Attributes.Deck.OnDeckRefilled -= DeckRefilledHandler;
            hasRegisteredDeckEvents = false;
        }
    }
}
