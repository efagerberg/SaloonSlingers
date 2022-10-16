using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Unity.Slingers;

namespace SaloonSlingers.Unity.CardEntities
{
    public class DeckGraphic : MonoBehaviour
    {
        public Transform TopCardTransform { get => cardGraphics.Peek().transform; }

        [SerializeField]
        private GameObject handInteractablePrefab;

        private ICardSpawner cardSpawner;
        private const float zOffset = 0.001f;
        private readonly Stack<ICardGraphic> cardGraphics = new();
        private ISlingerAttributes slingerAttributes;

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
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
            SpawnDeck();
        }

        private void SpawnDeck()
        {
            for (int i = 0; i < slingerAttributes.Deck.Count; i++)
            {
                ICardGraphic cardGraphic = cardSpawner.Spawn();
                cardGraphic.transform.SetParent(transform, false);
                cardGraphic.transform.localPosition = GetLocalPositionOfCard(i);
                cardGraphics.Push(cardGraphic);
            }
        }
    }
}
