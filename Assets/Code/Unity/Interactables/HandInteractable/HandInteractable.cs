using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.Interactables
{
    public class HandInteractable : XRGrabInteractable
    {
        [SerializeField]
        private GameObject slingerGO;
        [SerializeField]
        private CardSpawner cardSpawner;
        [SerializeField]
        private GameRulesManager gameRulesManager;
        [SerializeField]
        private RectTransform handPanelRectTransform;
        [SerializeField]
        private RectTransform handCanvasRectTransform;
        [SerializeField]
        private float totalCardDegrees = 30f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;

        private TrailRenderer trailRenderer;
        private Rigidbody rigidBody;
        private ThrowState throwState;
        private HandLayoutMediator handLayoutMediator;
        private ISlingerAttributes slingerAttributes;
        private Func<int, IEnumerable<float>> cardRotationCalculator;
        private bool isHandCommitted = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            throwState = new();
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            commitHandActionProperties.ForEach(prop => prop.action.started += ToggleCommitHand);
            throwState = throwState.Reset();
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            commitHandActionProperties.ForEach(prop => prop.action.started -= ToggleCommitHand);
            throwState = throwState.Throw();
            NegateCharacterControllerVelocity(args.interactorObject);
        }

        private void ToggleCommitHand(InputAction.CallbackContext _)
        {
            isHandCommitted = !isHandCommitted;
            handLayoutMediator.ApplyLayout(isHandCommitted, cardRotationCalculator);
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            TryAddCard();
        }

        private void TryAddCard()
        {
            if (isHandCommitted || slingerAttributes.Hand.Count >= gameRulesManager.GameRules.MaxHandSize) return;

            Card card = slingerAttributes.Deck.Dequeue();
            slingerAttributes.Hand.Add(card);
            ICardGraphic cardGraphic = cardSpawner.Spawn(card);
            handLayoutMediator.AddCardToLayout(cardGraphic, cardRotationCalculator);
        }

        private void Start()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            slingerAttributes = slingerGO.GetComponent<ISlinger>().Attributes;
            if (handCanvasRectTransform == null) handCanvasRectTransform = handPanelRectTransform.parent.GetComponent<RectTransform>();

            handLayoutMediator = new(handPanelRectTransform, handCanvasRectTransform);
            TryAddCard();
        }

        private void NegateCharacterControllerVelocity(IXRInteractor interactorObject)
        {
            var c = interactorObject.transform.GetComponentInParent<CharacterController>();
            rigidBody.AddForce(-c.velocity);
        }

        private void FixedUpdate()
        {
            throwState = throwState.Update(rigidBody.velocity.magnitude == 0);
            if (throwState.IsAlive) return;

            handLayoutMediator.Dispose(cardSpawner.Despawn);
            slingerAttributes.Hand.Clear();
            Destroy(gameObject);
        }
    }
}