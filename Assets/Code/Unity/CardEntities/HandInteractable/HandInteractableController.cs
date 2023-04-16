using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.CardEntities
{
    public class HandInteractableController : MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;

        private CardHand cardHand;
        private DeckGraphic deckGraphic;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            cardHand.AssignNewSlinger(args.interactorObject.transform);
            cardHand.Pickup(deckGraphic.Spawn);
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
            cardHand.TryDrawCard(deckGraphic.Spawn);
        }

        private bool IsTouchingDeck()
        {
            if (!deckGraphic.CanDraw) return false;

            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphic.TopCardTransform.position));
            return dist <= maxDeckDistance;
        }

        private void Start()
        {
            cardHand = GetComponent<CardHand>();
            deckGraphic = GameObject.FindGameObjectWithTag("DeckGraphic").GetComponent<DeckGraphic>();
        }

        private void OnToggleCommit(InputAction.CallbackContext _) => cardHand.ToggleCommitHand();
    }
}