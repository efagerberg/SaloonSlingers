using System.Collections.Generic;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.CardEntities
{
    public class PlayerHandInteractableController : MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;

        private HandProjectile handProjectile;
        private DeckGraphic deckGraphic;
        private Vector3 slingerVelocityAtRelease;
        private Rigidbody rb;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            XROrigin origin = args.interactorObject.transform.GetComponentInParent<XROrigin>();
            SlingerHandedness handedness = origin.GetComponent<SlingerHandedness>();
            deckGraphic = handedness.DeckGraphic;
            handProjectile.AssignDeck(deckGraphic.Deck);
            handProjectile.Pickup(deckGraphic.Spawn);
            commitHandActionProperties.ForEach(prop => prop.action.started += OnToggleCommit);
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            CharacterController characterController = args.interactorObject.transform.root.GetComponentInChildren<CharacterController>();
            slingerVelocityAtRelease = characterController.velocity;
            handProjectile.Throw();
            commitHandActionProperties.ForEach(prop => prop.action.started -= OnToggleCommit);
        }

        public void OnActivate(ActivateEventArgs _)
        {
            if (!deckGraphic.CanDraw || !IsTouchingDeck()) return;
            handProjectile.TryDrawCard(deckGraphic.Spawn);
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphic.TopCardTransform.position));
            return dist <= maxDeckDistance;
        }

        private void Start()
        {
            handProjectile = GetComponent<HandProjectile>();
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (handProjectile.IsThrown)
                rb.AddForce(-slingerVelocityAtRelease * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        private void OnToggleCommit(InputAction.CallbackContext _) => handProjectile.ToggleCommitHand();
    }
}