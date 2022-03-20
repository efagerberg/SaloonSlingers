using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using SaloonSlingers.Core.SlingerAttributes;
using System;
using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.HandInteractable
{
    public class HandInteractable : XRGrabInteractable
    {
        [SerializeField]
        private GameObject slingerGO;
        [SerializeField]
        private CardSpawner cardSpawner;
        [SerializeField]
        private float maxLifetime = 2f;
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
        private Func<int, IEnumerable<float>> rotationCalculator;

        protected override void OnEnable()
        {
            base.OnEnable();
            rotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            throwState = new(maxLifetime);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            commitHandActionProperties.ForEach(prop => prop.action.started += CommitHand);
            throwState = throwState.Reset();
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            commitHandActionProperties.ForEach(prop => prop.action.started -= CommitHand);
            throwState = throwState.Throw();
            NegateCharacterControllerVelocity(args.interactorObject);
        }

        private void CommitHand(InputAction.CallbackContext ctx)
        {
            handLayoutMediator.ToggleCommitHand(rotationCalculator, ctx);
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            handLayoutMediator.AddCardToHand(gameRulesManager.GameRules.MaxHandSize, slingerAttributes, cardSpawner.Spawn, rotationCalculator);
        }

        private void Start()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            slingerAttributes = slingerGO.GetComponent<ISlinger>().Attributes;
            if (handCanvasRectTransform == null) handCanvasRectTransform = handPanelRectTransform.parent.GetComponent<RectTransform>();

            handLayoutMediator = new(handPanelRectTransform, handCanvasRectTransform, new List<ITangibleCard>());
            handLayoutMediator.AddCardToHand(gameRulesManager.GameRules.MaxHandSize, slingerAttributes, cardSpawner.Spawn, rotationCalculator);
        }

        private void NegateCharacterControllerVelocity(IXRInteractor interactorObject)
        {
            var c = interactorObject.transform.GetComponentInParent<CharacterController>();
            rigidBody.AddForce(-c.velocity);
        }

        private void FixedUpdate()
        {
            throwState = throwState.Update(rigidBody.velocity.magnitude, Time.deltaTime);
            if (!throwState.IsAlive)
            {
                handLayoutMediator.Dispose(cardSpawner.Despawn, slingerAttributes.Hand);
                Destroy(gameObject);
            }
        }
    }
}