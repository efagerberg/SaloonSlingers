using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core;


namespace SaloonSlingers.Unity
{
    public class DeckGraphic : MonoBehaviour
    {
        private CardSpawner cardSpawner;
        private Deck deck;
        private const float zOffset = 0.001f;
        private readonly Stack<ICardGraphic> cardGraphics = new();

        public ICardGraphic Draw() => cardGraphics.Pop();

        public void Return(ICardGraphic cardGraphic)
        {
            cardGraphic.transform.SetParent(transform);
            cardGraphic.transform.position = GetLocalPositionOfCard(cardGraphics.Count);
            cardGraphics.Push(cardGraphic);
        }

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
            deck = GetComponentInParent<ISlinger>().Attributes.Deck;
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<CardSpawner>();
            SpawnDeck();
        }

        private void SpawnDeck()
        {
            for (int i = 0; i < deck.Count; i++)
            {
                ICardGraphic cardGraphic = cardSpawner.Spawn();
                cardGraphic.transform.SetParent(transform, false);
                cardGraphic.transform.localPosition = GetLocalPositionOfCard(i);
                cardGraphics.Push(cardGraphic);
            }
        }
    }
}
