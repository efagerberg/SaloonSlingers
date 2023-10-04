using System;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public delegate void HandProjectileHeld(HandProjectile sender, EventArgs e);

    public class HandProjectile : MonoBehaviour, IActor
    {
        public event HandProjectileHeld HandProjectileHeld;
        public event EventHandler Death;
        public IList<ICardGraphic> CardGraphics { get; private set; } = new List<ICardGraphic>();
        public IList<Card> Cards { get => CardGraphics.Select(x => x.Card).ToList(); }
        public HandEvaluation HandEvaluation
        {
            get
            {
                if (requiresEvaluation)
                {
                    handEvaluation = gameManager.Saloon.HouseGame.Evaluate(Cards);
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
        private GameManager gameManager;
        private HandEvaluation handEvaluation;
        private bool requiresEvaluation = false;
        private DrawContext drawCtx;

        public void Pickup(Func<GameObject> spawnCard)
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            bool committedBefore = state.IsCommitted;
            state = state.Reset();
            if (committedBefore != state.IsCommitted)
                handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            if (CardGraphics.Count == 0) TryDrawCard(spawnCard);
            gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
            HandProjectileHeld?.Invoke(this, EventArgs.Empty);
        }

        public void TryDrawCard(Func<GameObject> spawnCard)
        {
            drawCtx.Deck = deck;
            drawCtx.Evaluation = HandEvaluation;
            drawCtx.Hand = Cards;
            bool canDraw = (
                !state.IsCommitted &&
                gameManager.Saloon.HouseGame.CanDraw(drawCtx)
            );
            if (!canDraw) return;

            Card card = deck.Draw();
            audioSource.clip = drawSFX;
            audioSource.Play();
            GameObject spawned = spawnCard();
            ICardGraphic cardGraphic = spawned.GetComponent<ICardGraphic>();
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

        public void Throw(Vector3 offset)
        {
            Throw();
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
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

        public void Reset()
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            handLayoutMediator.Reset();
            CardGraphics.Clear();
            requiresEvaluation = true;
            gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
        }

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }

        private void OnEnable()
        {
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            state = new(lifespanInSeconds);
        }

        private void Awake()
        {
            gameManager = GameManager.Instance;
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            handLayoutMediator = new(handPanelRectTransform);
        }

        private void FixedUpdate()
        {
            state.Update(Time.fixedDeltaTime);
            if (state.IsAlive) return;

            Kill();
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool targetHasHitPoints = collision.gameObject.TryGetComponent(out HitPoints targetHitPoints);
            bool targetHasTempHitPoints = collision.gameObject.TryGetComponent(out TemporaryHitPoints targetTempHitPoints);

            if (targetHasHitPoints)
            {
                if (targetHasTempHitPoints && targetTempHitPoints.Points.Value > 0)
                {
                    bool shouldDamageHitPoints = targetTempHitPoints.Points.Value < HandEvaluation.Score;
                    targetTempHitPoints.Points.Value = shouldDamageHitPoints ? 0 : targetTempHitPoints.Points.Value - HandEvaluation.Score;
                    if (shouldDamageHitPoints) targetHitPoints.Points.Value--;
                }
                else targetHitPoints.Points.Value--;
            }

            Kill();
        }
    }
}
