using System;
using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Unity.CardEntities;
using SaloonSlingers.Unity.Slingers;

namespace SaloonSlingers.Unity
{
    public class HandInteractablePlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject handInteractablePrefab;

        private Queue<CardHand> interactables = new(2);
        private ISlingerAttributes slingerAttributes;
        private ICardSpawner cardSpawner;
        private DeckGraphic deckGraphic;

        private void Start()
        {
            slingerAttributes = GetComponentInParent<ISlinger>().Attributes;
            deckGraphic = GetComponent<DeckGraphic>();
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
            InitializeInteractables();
        }

        private void PlaceOnTop(Transform topCardTransform)
        {
            if (interactables.Count == 0) return;
            CardHand interactable = interactables.Peek();
            interactable.transform.SetPositionAndRotation(topCardTransform.position, topCardTransform.rotation);
            interactable.transform.SetParent(transform);
        }

        private void HandInteractableHeldHandler(CardHand _, EventArgs __)
        {
            if (interactables.Count == 0) return;
            CardHand interactable = interactables.Dequeue();
            interactable.transform.SetParent(null);
            PlaceOnTop(deckGraphic.TopCardTransform);
        }

        private void HandInteractableReadyToRespawn(CardHand sender, EventArgs _)
        {
            interactables.Enqueue(sender);
            PlaceOnTop(deckGraphic.TopCardTransform);
        }

        private void InitializeInteractables()
        {
            GameObject primaryInteractableGO = Instantiate(handInteractablePrefab);
            CardHand primaryInteractable = primaryInteractableGO.GetComponent<CardHand>();
            primaryInteractable.AssociateWithSlinger(slingerAttributes, cardSpawner);
            primaryInteractable.OnHandInteractableHeld += HandInteractableHeldHandler;
            primaryInteractable.OnHandInterableReadyToRespawn += HandInteractableReadyToRespawn;
            interactables.Enqueue(primaryInteractable);
            PlaceOnTop(deckGraphic.TopCardTransform);

            GameObject secondardyInteractableGO = Instantiate(handInteractablePrefab);
            CardHand secondardyInteractable = secondardyInteractableGO.GetComponent<CardHand>();
            secondardyInteractable.AssociateWithSlinger(slingerAttributes, cardSpawner);
            secondardyInteractable.OnHandInteractableHeld += HandInteractableHeldHandler;
            secondardyInteractable.OnHandInterableReadyToRespawn += HandInteractableReadyToRespawn;
            interactables.Enqueue(secondardyInteractable);
        }
    }
}
