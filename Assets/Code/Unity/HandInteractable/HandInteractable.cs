using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


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
        private GameObject handPanel;
        [SerializeField]
        private float totalCardDegrees = 30f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;

        private TrailRenderer trailRenderer;
        private Rigidbody rigidBody;
        private ThrowState throwState;
        private HandLayoutMediator handLayoutMediator;

        protected override void OnEnable()
        {
            base.OnEnable();
            throwState = new(maxLifetime);
            if (handLayoutMediator == null) return;
            commitHandActionProperties.ForEach(prop => prop.action.started += handLayoutMediator.ToggleCommitHand);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            commitHandActionProperties.ForEach(prop => prop.action.started -= handLayoutMediator.ToggleCommitHand);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            throwState.SetUnthrown();
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            throwState.SetThrown();
            NegateCharacterControllerVelocity(args.interactorObject);
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            handLayoutMediator.AddCardToHand(gameRulesManager);
        }

        private void Start()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            handLayoutMediator = new(slingerGO, handPanel, cardSpawner, totalCardDegrees);
            handLayoutMediator.AddCardToHand(gameRulesManager);
            commitHandActionProperties.ForEach(prop => prop.action.started += handLayoutMediator.ToggleCommitHand);
        }

        private void NegateCharacterControllerVelocity(IXRInteractor interactorObject)
        {
            var c = interactorObject.transform.GetComponentInParent<CharacterController>();
            rigidBody.AddForce(-c.velocity);
        }

        private void FixedUpdate()
        {
            throwState.Update(rigidBody.velocity.magnitude, Time.deltaTime);
            if (!throwState.IsAlive)
            {
                handLayoutMediator.Dispose();
                Destroy(gameObject);
            }
        }
    }
}