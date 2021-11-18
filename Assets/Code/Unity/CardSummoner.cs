using UnityEngine;

using GambitSimulator.Core;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;

namespace GambitSimulator.Unity
{
    [RequireComponent(typeof(XRDirectInteractor))]
    [RequireComponent(typeof(ActionBasedController))]
    public class CardSummoner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private XRInteractionManager interactionManager;

        private XRDirectInteractor interactor;
        private Deck deck = new Deck(3).Shuffle();
        private ActionBasedController controller;

        private void OnEnable()
        {
            controller = GetComponent<ActionBasedController>();
            interactor = GetComponent<XRDirectInteractor>();
            controller.selectAction.action.performed += SpawnCard;
        }

        private void OnDisable()
        {
            controller.selectAction.action.performed -= SpawnCard;
        }

        private void SpawnCard(InputAction.CallbackContext obj)
        {
            Card card = deck.RemoveFromTop();
            var go = Instantiate(cardPrefab);
            var cardComponent = go.GetComponent<CardInteractable>();
            cardComponent.SetCard(card);
            interactionManager.ForceSelect(interactor, cardComponent);
        }
    }
}
