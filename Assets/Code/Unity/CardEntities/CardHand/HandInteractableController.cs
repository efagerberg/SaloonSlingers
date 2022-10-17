using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.CardEntities
{
    public class HandInteractableController: MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;

        private CardHand cardHand;
        private Transform deckGraphicTransform;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (!IsTouchingDeck()) return;
            cardHand.SwapHandIfDifferentSlinger(args.interactorObject.transform);
            cardHand.Pickup();
            commitHandActionProperties.ForEach(prop => prop.action.started += OnToggleCommit);
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            Rigidbody characterControllerRb = args.interactorObject.transform.GetComponentInParent<Rigidbody>();
            cardHand.Throw(characterControllerRb);
            commitHandActionProperties.ForEach(prop => prop.action.started -= OnToggleCommit);
        }

        public void OnActivate(ActivateEventArgs _)
        {
            if (!IsTouchingDeck()) return;
            cardHand.TryDrawCard();
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphicTransform.transform.position));
            return dist <= maxDeckDistance;
        }

        private void Start()
        {
            deckGraphicTransform = GameObject.FindGameObjectWithTag("DeckGraphic").transform;
            cardHand = GetComponent<CardHand>();
        }

        private void OnToggleCommit(InputAction.CallbackContext _) => cardHand.ToggleCommitHand();
    }
}