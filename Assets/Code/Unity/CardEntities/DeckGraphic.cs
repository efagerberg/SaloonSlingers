using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public delegate void DeckGraphicEmptyHandler(DeckGraphic sender, EventArgs e);

    public class DeckGraphic : MonoBehaviour, ICardSpawner
    {
        public Transform TopCardTransform { get => cardGraphics.Peek().transform; }
        public bool CanDraw { get; private set; }
        public event DeckGraphicEmptyHandler OnDeckGraphicEmpty;
        public Deck Deck { get; private set; }

        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;

        private ICardSpawner cardSpawner;
        private const float zOffset = 0.001f;
        private readonly Stack<ICardGraphic> cardGraphics = new();

        public ICardGraphic Spawn() => cardGraphics.Pop();
        public void Despawn(ICardGraphic cardGraphic) => cardSpawner.Despawn(cardGraphic);

        private Vector3 GetLocalPositionOfCard(int i)
        {
            return new(
                transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z + zOffset * i
            );
        }

        private void Awake()
        {
            Deck = new Deck(numberOfCards).Shuffle();
        }

        private void OnDisable()
        {
            Deck.OnDeckEmpty -= DeckEmptyHandler;
        }

        private void OnEnable()
        {
            Deck.OnDeckEmpty += DeckEmptyHandler;
        }

        private void Start()
        {
            Deck.OnDeckEmpty += DeckEmptyHandler;
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
            SpawnDeck();
        }

        private void SpawnDeck()
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                CanDraw = true;
                ICardGraphic cardGraphic = cardSpawner.Spawn();
                cardGraphic.transform.SetParent(transform, false);
                cardGraphic.transform.localPosition = GetLocalPositionOfCard(i);
                cardGraphics.Push(cardGraphic);
            }
        }

        private void DeckEmptyHandler(Deck _, EventArgs __)
        {
            CanDraw = false;
            OnDeckGraphicEmpty?.Invoke(this, EventArgs.Empty);
        }
    }
}
