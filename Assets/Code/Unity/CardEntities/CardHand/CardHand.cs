using System;
using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Unity.Slingers;

namespace SaloonSlingers.Unity.CardEntities
{
    public delegate void HandInteractableHeld(CardHand sender, EventArgs e);
    public delegate void HandInteractableReadyToRespawn(CardHand sender, EventArgs e);

    public class CardHand : MonoBehaviour
    {
        public event HandInteractableHeld OnHandInteractableHeld;
        public event HandInteractableReadyToRespawn OnHandInteractableDied;
        public IList<ICardGraphic> Cards { get; private set; }

        [SerializeField]
        private RectTransform handPanelRectTransform;
        [SerializeField]
        private RectTransform handCanvasRectTransform;
        [SerializeField]
        private float totalCardDegrees = 30f;
        [SerializeField]
        private float lifespanInSeconds = 1f;
        [SerializeField]
        private int maxAngularVelocity = 100;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip drawSFX;
        [SerializeField]
        private AudioClip throwSFX;

        private TrailRenderer trailRenderer;
        private Rigidbody rigidBody;
        private CardHandState state;
        private CardHandLayoutMediator handLayoutMediator;
        private Func<int, IEnumerable<float>> cardRotationCalculator;
        private ISlingerAttributes slingerAttributes;
        private GameRulesManager gameRulesManager;
        private float originlLifespanInSeconds;

        public void Pickup(Func<ICardGraphic> spawnCard)
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            lifespanInSeconds = originlLifespanInSeconds;
            if (Cards.Count == 0) TryDrawCard(spawnCard);
            OnHandInteractableHeld?.Invoke(this, EventArgs.Empty);
        }

        public void TryDrawCard(Func<ICardGraphic> spawnCard)
        {
            if (!state.CanDraw) return;

            Card card = slingerAttributes.Deck.RemoveFromTop();
            audioSource.clip = drawSFX;
            audioSource.Play();
            ICardGraphic cardGraphic = spawnCard();
            cardGraphic.Card = card;
            Cards.Add(cardGraphic);
            handLayoutMediator.AddCardToLayout(cardGraphic, cardRotationCalculator);
        }

        public void Throw(Rigidbody characterControllerRb)
        {
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            state = state.Throw();
            audioSource.clip = throwSFX;
            audioSource.Play();
            lifespanInSeconds = originlLifespanInSeconds;
            NegateCharacterControllerVelocity(characterControllerRb);
        }

        public void SwapHandIfDifferentSlinger(Transform slingerTransform)
        {
            ISlingerAttributes newAttributes = slingerTransform.GetComponentInParent<ISlinger>().Attributes;
            if (slingerAttributes == null || slingerAttributes != newAttributes)
            {
                AssociateWithSlinger(newAttributes);
                return;
            }
        }

        public void ToggleCommitHand()
        {
            state = state.ToggleCommit();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
        }

        private void AssociateWithSlinger(ISlingerAttributes attributes)
        {
            slingerAttributes = attributes;
        }

        private void OnEnable()
        {
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            state = new(
                () => lifespanInSeconds > 0 && rigidBody.velocity.magnitude != 0,
                () => Cards.Count < gameRulesManager.GameRules.MaxHandSize &&
                      slingerAttributes.Deck.Count > 0
            );
        }

        private void Start()
        {
            Cards = new List<ICardGraphic>();
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            if (handCanvasRectTransform == null) handCanvasRectTransform = handPanelRectTransform.parent.GetComponent<RectTransform>();

            handLayoutMediator = new(handPanelRectTransform, handCanvasRectTransform);
            originlLifespanInSeconds = lifespanInSeconds;
        }

        private void NegateCharacterControllerVelocity(Rigidbody characterControllerRb)
        {
            if (characterControllerRb == null) return;
            rigidBody.AddForce(-characterControllerRb.velocity);
        }

        private void FixedUpdate()
        {
            if (state.IsThrown) lifespanInSeconds -= Time.fixedDeltaTime;
            if (state.IsAlive) return;

            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            Cards.Clear();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            handLayoutMediator.Dispose();
            lifespanInSeconds = originlLifespanInSeconds;
            OnHandInteractableDied?.Invoke(this, EventArgs.Empty);
        }
    }
}