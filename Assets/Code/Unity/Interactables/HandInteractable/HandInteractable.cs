using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.Interactables
{
    public delegate void HandInteractableHeld(HandInteractable sender, EventArgs e);
    public delegate void HandInteractableReadyToRespawn(HandInteractable sender, EventArgs e);

    public class HandInteractable : XRGrabInteractable
    {
        public event HandInteractableHeld OnHandInteractableHeld;
        public event HandInteractableReadyToRespawn OnHandInterableReadyToRespawn;
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
        private Func<int, IEnumerable<float>> cardRotationCalculator;
        private bool isHandCommitted = false;
        private ISlingerAttributes slingerAttributes;
        private ICardSpawner cardSpawner;
        private GameRulesManager gameRulesManager;

        public void AssociateWithSlinger(ISlingerAttributes attributes, ICardSpawner slingerCardSpawner)
        {
            slingerAttributes = attributes;
            cardSpawner = slingerCardSpawner;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            throwState = new();
        }

        private void Start()
        {
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            if (handCanvasRectTransform == null) handCanvasRectTransform = handPanelRectTransform.parent.GetComponent<RectTransform>();

            handLayoutMediator = new(handPanelRectTransform, handCanvasRectTransform);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            commitHandActionProperties.ForEach(prop => prop.action.started += ToggleCommitHand);
            throwState = throwState.Reset();
            if (slingerAttributes.Hand.Count == 0) TryAddCard();
            OnHandInteractableHeld?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            commitHandActionProperties.ForEach(prop => prop.action.started -= ToggleCommitHand);
            throwState = throwState.Throw();
            slingerAttributes.Hand.Clear();
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
            if (!CanDraw) return;

            Card card = slingerAttributes.Deck.Dequeue();
            slingerAttributes.Hand.Add(card);
            ICardGraphic cardGraphic = cardSpawner.Spawn(card);
            handLayoutMediator.AddCardToLayout(cardGraphic, cardRotationCalculator);
        }

        private bool CanDraw
        {
            get
            {
                return !isHandCommitted && slingerAttributes.Hand.Count < gameRulesManager.GameRules.MaxHandSize && slingerAttributes.Deck.Count > 0;
            }
        }

        private void NegateCharacterControllerVelocity(IXRInteractor interactorObject)
        {
            CharacterController c = interactorObject.transform.GetComponentInParent<CharacterController>();
            rigidBody.AddForce(-c.velocity);
        }

        private void FixedUpdate()
        {
            throwState = throwState.Update(rigidBody.velocity.magnitude == 0);
            if (throwState.IsAlive) return;

            handLayoutMediator.Dispose(cardSpawner.Despawn);
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            throwState = throwState.Reset();
            if (isHandCommitted) ToggleCommitHand(new InputAction.CallbackContext());
            OnHandInterableReadyToRespawn?.Invoke(this, EventArgs.Empty);
        }
    }
}