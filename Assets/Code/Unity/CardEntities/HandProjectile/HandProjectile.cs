using System;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public delegate void HandProjectileHeld(HandProjectile sender, EventArgs e);
    public delegate void HandProjectileReadyToRespawn(HandProjectile sender, EventArgs e);

    public class HandProjectile : MonoBehaviour
    {
        public event HandProjectileHeld OnHandInteractableHeld;
        public event HandProjectileReadyToRespawn OnHandInteractableDied;
        public IList<ICardGraphic> CardGraphics { get; private set; }
        public IList<Card> Cards { get => CardGraphics.Select(x => x.Card).ToList(); }

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
        private HandProjectileState state;
        private HandLayoutMediator handLayoutMediator;
        private Func<int, IEnumerable<float>> cardRotationCalculator;
        private Deck deck;
        private GameRulesManager gameRulesManager;

        public void Pickup(Func<ICardGraphic> spawnCard)
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            bool wasCommitted = state.IsCommitted;
            state = state.Reset();
            if (wasCommitted)
                handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            if (CardGraphics.Count == 0) TryDrawCard(spawnCard);
            OnHandInteractableHeld?.Invoke(this, EventArgs.Empty);
        }

        public void TryDrawCard(Func<ICardGraphic> spawnCard)
        {
            if (!state.CanDraw) return;

            Card card = deck.RemoveFromTop();
            audioSource.clip = drawSFX;
            audioSource.Play();
            ICardGraphic cardGraphic = spawnCard();
            cardGraphic.Card = card;
            CardGraphics.Add(cardGraphic);
            handLayoutMediator.AddCardToLayout(cardGraphic, cardRotationCalculator);
        }

        public void Throw()
        {
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            state = state.Throw();
            audioSource.clip = throwSFX;
            audioSource.Play();
        }

        public void AssignDeck(Deck newDeck)
        {
            if (deck != null || deck != newDeck)
                deck = newDeck;
        }

        public void ToggleCommitHand()
        {
            state = state.ToggleCommit();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
        }

        public bool IsThrown { get => state.IsThrown; }

        private void OnEnable()
        {
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            state = new(
                lifespanInSeconds,
                () => CardGraphics.Count < gameRulesManager.GameRules.MaxHandSize &&
                      deck.Count > 0
            );
        }

        private void Awake()
        {
            CardGraphics = new List<ICardGraphic>();
        }

        private void Start()
        {
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            if (handCanvasRectTransform == null) handCanvasRectTransform = handPanelRectTransform.parent.GetComponent<RectTransform>();

            handLayoutMediator = new(handPanelRectTransform, handCanvasRectTransform);
        }

        private void FixedUpdate()
        {
            state.Update(Time.fixedDeltaTime);
            if (state.IsAlive) return;

            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            handLayoutMediator.Dispose();
            OnHandInteractableDied?.Invoke(this, EventArgs.Empty);
            CardGraphics.Clear();
        }
    }
}