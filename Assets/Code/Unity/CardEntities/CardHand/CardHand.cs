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
        public event HandInteractableReadyToRespawn OnHandInterableReadyToRespawn;

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
        private ICardSpawner cardSpawner;
        private GameRulesManager gameRulesManager;
        private float originlLifespanInSeconds;
        private IList<Card> cards = new List<Card>();

        public void Pickup()
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            lifespanInSeconds = originlLifespanInSeconds;
            if (cards.Count == 0) TryDrawCard();
            OnHandInteractableHeld?.Invoke(this, EventArgs.Empty);
        }

        public void TryDrawCard()
        {
            if (!state.CanDraw) return;

            Card card = slingerAttributes.Deck.Dequeue();
            cards.Add(card);
            audioSource.clip = drawSFX;
            audioSource.Play();
            ICardGraphic cardGraphic = cardSpawner.Spawn();
            cardGraphic.Card = card;
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
            if (slingerAttributes == null)
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
                () => cards.Count < gameRulesManager.GameRules.MaxHandSize &&
                      slingerAttributes.Deck.Count > 0
            );
        }

        private void Start()
        {
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
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
            cards.Clear();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            handLayoutMediator.Dispose(cardSpawner.Despawn);
            lifespanInSeconds = originlLifespanInSeconds;
            OnHandInterableReadyToRespawn?.Invoke(this, EventArgs.Empty);
        }
    }
}