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
        public event HandProjectileHeld OnHandProjectileHeld;
        public event HandProjectileReadyToRespawn OnHandProjectileDied;
        public IList<ICardGraphic> CardGraphics { get; private set; }
        public IList<Card> Cards { get => CardGraphics.Select(x => x.Card).ToList(); }
        public HandEvaluation HandEvaluation
        {
            get
            {
                if (requiresEvaluation)
                {
                    handEvaluation = gameRulesManager.GameRules.HandEvaluator.Evaluate(Cards);
                    requiresEvaluation = false;
                }
                return handEvaluation;
            }
        }

        [SerializeField]
        private RectTransform handPanelRectTransform;
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
        private HandEvaluation handEvaluation;
        private bool requiresEvaluation = false;

        public void Pickup(Func<ICardGraphic> spawnCard)
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            bool committedBefore = state.IsCommitted;
            state = state.Reset();
            if (committedBefore != state.IsCommitted)
                handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            if (CardGraphics.Count == 0) TryDrawCard(spawnCard);
            OnHandProjectileHeld?.Invoke(this, EventArgs.Empty);
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
            requiresEvaluation = true;
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

            handLayoutMediator = new(handPanelRectTransform);
        }

        private void FixedUpdate()
        {
            state.Update(Time.fixedDeltaTime);
            if (state.IsAlive) return;

            Reset();
        }

        private void Reset()
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            handLayoutMediator.Dispose();
            OnHandProjectileDied?.Invoke(this, EventArgs.Empty);
            CardGraphics.Clear();
            requiresEvaluation = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out Health targetHealth)) return;

            HandProjectile[] targetProjectiles = collision.gameObject.GetComponentsInChildren<HandProjectile>();
            if (targetProjectiles.Length == 0)
            {
                targetHealth.Points.Value--;
                return;
            };

            // If target has any hand better than this instance, they should not lose health;
            bool targetHasSuperiorHand = false;
            foreach (var targetProjectile in targetProjectiles)
            {
                if (HandEvaluation.Score < targetProjectile.HandEvaluation.Score)
                {
                    targetHasSuperiorHand = true;
                    break;
                }
            }
            if (!targetHasSuperiorHand) targetHealth.Points.Value--;

            Reset();
        }
    }
}