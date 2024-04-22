﻿using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;


namespace SaloonSlingers.Unity.Actor
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Homable))]
    [RequireComponent(typeof(HandProjectile))]
    public class PlayerHandInteractableController : MonoBehaviour
    {
        public UnityEvent<GameObject> OnPreDrawHoverEnter = new();
        public UnityEvent<GameObject> OnPreDrawHoverExit = new();

        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private XRBaseInteractable mainInteractable;
        [SerializeField]
        private XRBaseInteractable peerInteractable;

        private HandProjectile handProjectile;
        private DeckGraphic deckGraphic;
        private Homable homable;

        private Rigidbody rb;
        private HomingStrength homingStrength;
        private CharacterControllerThrowOffsetCalculator throwOffsetCalculator;
        private CharacterController characterController;
        private VisibilityDetector visibilityDetector;
        private bool initialized = false;
        private bool eventsRegistered = false;
        private static int ENUM_COUNT = Enum.GetValues(typeof(HandProjectileMode)).Length;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor)))
                return;

            var player = LevelManager.Instance.Player;
            int newSlingerId = player.transform.GetInstanceID();
            if (!initialized) Initialize(player);

            handProjectile.Pickup(deckGraphic.Spawn, GameManager.Instance.Saloon.HouseGame);
            handProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
            peerInteractable.enabled = true;
            homable.enabled = true;
            if (!eventsRegistered)
            {
                handProjectile.OnKilled.AddListener(OnHandProjectileDied);
                eventsRegistered = true;
            }
        }

        /// <summary>
        /// Sets up the expensive to initialize state once for an object even if it is disabled and renabled (pooling).
        /// </summary>
        /// <param name="player"></param>
        private void Initialize(GameObject player)
        {
            initialized = true;
            ActorHandedness handedness = player.GetComponent<ActorHandedness>();
            homingStrength = player.GetComponent<HomingStrength>();
            throwOffsetCalculator = new();
            characterController = player.GetComponent<CharacterController>();
            visibilityDetector = player.GetComponent<VisibilityDetector>();
            deckGraphic = handedness.DeckGraphic;
            var attributes = player.GetComponent<Attributes>();
            handProjectile = GetComponent<HandProjectile>();
            handProjectile.Assign(deckGraphic.Deck, attributes.Registry);
            rb = GetComponent<Rigidbody>();
            homable = GetComponent<Homable>();
            OnPreDrawHoverExit.Invoke(gameObject);
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor))) return;

            var asGrabbable = (XRGrabInteractable)args.interactableObject;
            Vector3 offset = throwOffsetCalculator.Calculate(asGrabbable.throwVelocityScale);
            handProjectile.Throw(offset);
            peerInteractable.enabled = false;

            var target = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"))
                                           .FirstOrDefault();
            if (target != null) homable.Target = target;

            homingStrength.Calculator.StartNewThrow();
        }

        public void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (initialized) return;

            OnPreDrawHoverEnter.Invoke(gameObject);
        }

        public void OnHoverExited(HoverExitEventArgs args)
        {
            if (initialized) return;

            OnPreDrawHoverExit.Invoke(gameObject);
        }

        private void OnHandProjectileDied(GameObject sender)
        {
            homable.Target = null;
            homable.Strength = 0;
            homable.enabled = false;
            eventsRegistered = false;
            handProjectile.OnKilled.RemoveListener(OnHandProjectileDied);
        }

        public void OnActivate()
        {
            if (deckGraphic.CanDraw && IsTouchingDeck())
            {
                handProjectile.TryDrawCard(deckGraphic.Spawn, GameManager.Instance.Saloon.HouseGame);
                return;
            }
            int nextEnumValue = ((int)handProjectile.Mode + 1) % ENUM_COUNT;
            handProjectile.Mode = (HandProjectileMode)nextEnumValue;
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphic.TopCardTransform.position));
            return dist <= maxDeckDistance;
        }

        private void OnEnable()
        {
            mainInteractable.enabled = true;
            peerInteractable.enabled = false;
        }

        private void OnDisable()
        {
            mainInteractable.enabled = false;
            peerInteractable.enabled = false;
            initialized = false;
        }

        private void FixedUpdate()
        {
            if (homable != null && homable.Target && homingStrength != null)
                homable.Strength = homingStrength.Calculator.Calculate(rb.angularVelocity.y);

            if (throwOffsetCalculator != null) throwOffsetCalculator.RecordVelocity(characterController.velocity);
        }
    }
}