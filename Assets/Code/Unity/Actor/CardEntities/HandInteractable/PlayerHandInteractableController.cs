﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHandInteractableController : MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private XRBaseInteractable mainInteractable;
        [SerializeField]
        private XRBaseInteractable peerInteractable;

        private HandProjectile handProjectile;
        private DeckGraphic deckGraphic;
        private Homable homable;
        private int? slingerId;

        private Rigidbody rb;
        private HomingStrengthCalculator homingStrengthCalculator;
        private CharacterControllerThrowOffsetCalculator throwOffsetCalculator;
        private VisibilityDetector visibilityDetector;
        private readonly HashSet<int> registeredProjectileIds = new();

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor))) return;

            var player = LevelManager.Instance.Player;
            int newSlingerId = player.transform.GetInstanceID();
            bool sameSlinger = newSlingerId == slingerId;
            if (slingerId == null || !sameSlinger)
            {
                slingerId = newSlingerId;
                ActorHandedness handedness = player.GetComponent<ActorHandedness>();
                homingStrengthCalculator = player.GetComponent<HomingStrengthCalculator>();
                throwOffsetCalculator = player.GetComponent<CharacterControllerThrowOffsetCalculator>();
                visibilityDetector = player.GetComponent<VisibilityDetector>();
                deckGraphic = handedness.DeckGraphic;
                handProjectile.AssignDeck(deckGraphic.Deck);
            }

            handProjectile.Pickup(deckGraphic.Spawn);
            handProjectile.transform.SetParent(args.interactorObject.transform);
            peerInteractable.enabled = true;
            homable.enabled = true;
            if (!registeredProjectileIds.Contains(handProjectile.GetInstanceID()))
            {
                handProjectile.Death += OnHandProjectileDied;
                registeredProjectileIds.Add(handProjectile.GetInstanceID());
            }
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor))) return;

            handProjectile.transform.parent = null;
            Vector3 offset = throwOffsetCalculator.Calculate((XRGrabInteractable)args.interactableObject);
            handProjectile.Throw(offset);
            peerInteractable.enabled = false;

            var target = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"))
                                           .FirstOrDefault();
            if (target != null) homable.Target = target;

            handProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
            homingStrengthCalculator.StartNewThrow();
        }


        private void OnHandProjectileDied(object sender, EventArgs e)
        {
            var instance = sender as GameObject;
            var actor = instance.GetComponent<IActor>();
            actor.Death -= OnHandProjectileDied;
            homable.Target = null;
            homable.Strength = 0;
            homable.enabled = false;
        }

        public void OnActivate()
        {
            if (!deckGraphic.CanDraw || !IsTouchingDeck()) return;

            handProjectile.TryDrawCard(deckGraphic.Spawn);
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphic.TopCardTransform.position));
            return dist <= maxDeckDistance;
        }

        private void Awake()
        {
            handProjectile = GetComponent<HandProjectile>();
            rb = GetComponent<Rigidbody>();
            homable = GetComponent<Homable>();
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
        }

        private void FixedUpdate()
        {
            if (homable.Target && homingStrengthCalculator != null)
                homable.Strength = homingStrengthCalculator.Calculate(rb.angularVelocity.y);

            if (throwOffsetCalculator) throwOffsetCalculator.RecordVelocity();
        }
    }
}