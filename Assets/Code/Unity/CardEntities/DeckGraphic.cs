using System;
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
        private Queue<CardHand> interactables = new(2);
        private ISlingerAttributes slingerAttributes;

        public ICardGraphic Spawn() => cardGraphics.Pop();
        public ICardGraphic Spawn(Card c)
        {
            ICardGraphic cardGraphic = Spawn();
            cardGraphic.Card = c;

            TryPlaceNextInteractableOnTop();
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

        private void TryPlaceNextInteractableOnTop()
        {
            if (interactables.Count == 0) return;
            if (cardGraphics.Count == 0) return;
            ICardGraphic topCardGraphic = cardGraphics.Peek();
            CardHand interactable = interactables.Peek();
            interactable.transform.SetPositionAndRotation(topCardGraphic.transform.position, topCardGraphic.transform.rotation);
            interactable.transform.SetParent(transform);
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

            GameObject primaryInteractableGO = Instantiate(handInteractablePrefab);
            CardHand primaryInteractable = primaryInteractableGO.GetComponent<CardHand>();
            primaryInteractable.AssociateWithSlinger(slingerAttributes, this);
            primaryInteractable.OnHandInteractableHeld += HandInteractableHeldHandler;
            primaryInteractable.OnHandInterableReadyToRespawn += HandInteractableReadyToRespawn;
            interactables.Enqueue(primaryInteractable);
            TryPlaceNextInteractableOnTop();

            GameObject secondardyInteractableGO = Instantiate(handInteractablePrefab);
            CardHand secondardyInteractable = secondardyInteractableGO.GetComponent<CardHand>();
            secondardyInteractable.AssociateWithSlinger(slingerAttributes, this);
            secondardyInteractable.OnHandInteractableHeld += HandInteractableHeldHandler;
            secondardyInteractable.OnHandInterableReadyToRespawn += HandInteractableReadyToRespawn;
            interactables.Enqueue(secondardyInteractable);
        }

        private void HandInteractableHeldHandler(CardHand _, EventArgs __)
        {
            if (interactables.Count == 0) return;
            CardHand interactable = interactables.Dequeue();
            interactable.transform.SetParent(null);
            TryPlaceNextInteractableOnTop();
        }

        private void HandInteractableReadyToRespawn(CardHand sender, EventArgs _)
        {
            interactables.Enqueue(sender);
            TryPlaceNextInteractableOnTop();
        }
    }
}
