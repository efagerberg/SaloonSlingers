using System;
using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Unity.Slingers;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public delegate void DeckGraphicEmptyHandler(DeckGraphic sender, EventArgs e);

    public class DeckGraphic : MonoBehaviour, ICardSpawner
    {
        public Transform TopCardTransform { get => cardGraphics.Peek().transform; }
        public bool CanDraw { get; private set; }
        public event DeckGraphicEmptyHandler OnDeckGraphicEmpty;

        [SerializeField]
        private GameObject handInteractablePrefab;

        private ICardSpawner cardSpawner;
        private const float zOffset = 0.001f;
        private readonly Stack<ICardGraphic> cardGraphics = new();
        private ISlingerAttributes slingerAttributes;

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

        private void Start()
        {
            slingerAttributes = GetComponentInParent<ISlinger>().Attributes;
            slingerAttributes.Deck.OnDeckEmpty += DeckEmptyHandler;
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
            SpawnDeck();
        }

        private void OnDisable()
        {
            if (slingerAttributes == null) return;
            slingerAttributes.Deck.OnDeckEmpty -= DeckEmptyHandler;
        }

        private void OnEnable()
        {
            if (slingerAttributes == null) return;
            slingerAttributes.Deck.OnDeckEmpty += DeckEmptyHandler;
        }

        private void SpawnDeck()
        {
            for (int i = 0; i < slingerAttributes.Deck.Count; i++)
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
