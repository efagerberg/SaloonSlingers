using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public delegate void DeckGraphicEmptyHandler(DeckGraphic sender, EventArgs e);

    public class DeckGraphic : MonoBehaviour
    {
        public Transform TopCardTransform { get => cardGraphics.Peek().transform; }
        public bool CanDraw { get; private set; }
        public Deck Deck { get; private set; }

        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;

        private CardSpawner cardSpawner;
        private const float zOffset = 0.001f;
        private readonly Stack<GameObject> cardGraphics = new();

        public GameObject Spawn() => cardGraphics.Pop();

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
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<CardSpawner>();
            SpawnDeck();
        }

        private void SpawnDeck()
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                var go = cardSpawner.Spawn();
                go.transform.SetParent(transform, false);
                go.transform.localPosition = GetLocalPositionOfCard(i);
                cardGraphics.Push(go);
            }
            CanDraw = true;
        }

        private void DeckEmptyHandler(Deck _, EventArgs __)
        {
            CanDraw = false;
        }
    }
}
