using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

using GambitSimulator.Core;

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
        [SerializeField]
        private PlayerAttributes playerAttributes;

        private XRDirectInteractor interactor;
        private Deck deck;
        private ActionBasedController controller;

        private void Start() => deck = playerAttributes.Deck;

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
            if (deck.Count == 0) return;
            Card card = deck.RemoveFromTop();
            var go = Instantiate(cardPrefab);
            var cardComponent = go.GetComponent<CardInteractable>();
            cardComponent.SetCard(card);
            interactionManager.ForceSelect(interactor, cardComponent);
        }
    }
}
