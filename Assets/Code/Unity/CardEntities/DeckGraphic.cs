using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Unity.Slingers;

namespace SaloonSlingers.Unity.CardEntities
{
    public class DeckGraphic : MonoBehaviour, ICardSpawner
    {
        [SerializeField]
        private GameObject handInteractablePrefab;
        private ICardSpawner baseCardSpawner;
        private const float zOffset = 0.001f;
        private readonly Stack<ICardGraphic> cardGraphics = new();
        private ISlingerAttributes slingerAttributes;

        public Transform TopCardTransform { get => cardGraphics.Peek().transform; }
        public ICardGraphic Spawn() => cardGraphics.Pop();
        public ICardGraphic Spawn(Card c)
        {
            ICardGraphic cardGraphic = Spawn();
            cardGraphic.Card = c;
            return cardGraphic;
        }

        public void Despawn(ICardGraphic cardGraphic) => baseCardSpawner.Despawn(cardGraphic);

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
            baseCardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
            SpawnDeck();
        }

        private void SpawnDeck()
        {
            for (int i = 0; i < slingerAttributes.Deck.Count; i++)
            {
                ICardGraphic cardGraphic = baseCardSpawner.Spawn();
                cardGraphic.transform.SetParent(transform, false);
                cardGraphic.transform.localPosition = GetLocalPositionOfCard(i);
                cardGraphics.Push(cardGraphic);
            }
        }
    }
}
