using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public delegate void HandProjectileHeld(HandProjectile sender, EventArgs e);

    public class HandProjectile : MonoBehaviour, IActor
    {
        public event HandProjectileHeld HandProjectileHeld;
        public event EventHandler Death;
        public IList<Card> Cards { get; private set; } = new List<Card>();
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
        private Collider _collider;

        public void Pickup(Func<GameObject> spawnCard)
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            _collider.isTrigger = true;
            bool stackedBefore = state.IsStacked;
            state = state.Reset();
            if (stackedBefore != state.IsStacked)
                handLayoutMediator.ApplyLayout(state.IsStacked, cardRotationCalculator);
            TryDrawCard(spawnCard);
            HandProjectileHeld?.Invoke(this, EventArgs.Empty);
        }

        public void TryDrawCard(Func<GameObject> spawnCard)
        {
            drawCtx.Deck = deck;
            drawCtx.Evaluation = HandEvaluation;
            drawCtx.Hand = Cards;
            if (!gameManager.Saloon.HouseGame.CanDraw(drawCtx)) return;

            Card card = deck.Draw();
            Cards.Add(card);
            audioSource.clip = drawSFX;
            audioSource.Play();
            GameObject spawned = spawnCard();
            ICardGraphic cardGraphic = spawned.GetComponent<ICardGraphic>();
            cardGraphic.Card = card;
            handLayoutMediator.AddCardToLayout(cardGraphic, cardRotationCalculator);
            requiresEvaluation = true;
        }

        public void Throw()
        {
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            _collider.isTrigger = false;
            Stack();
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

        public void Stack()
        {
            state = state.Stack();
            handLayoutMediator.ApplyLayout(state.IsStacked, cardRotationCalculator);
        }

        public void Unstack()
        {
            state = state.Unstack();
            handLayoutMediator.ApplyLayout(state.IsStacked, cardRotationCalculator);
        }

        public bool IsThrown { get => state.IsThrown; }

        public void Reset()
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            _collider.isTrigger = true;
            state = state.Reset();
            handLayoutMediator.Reset();
            Cards.Clear();
            requiresEvaluation = true;
            gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
        }

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }

        public void Pause()
        {
            state = state.Pause();
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            _collider.isTrigger = true;
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
            _collider = GetComponent<Collider>();
        }

        private void FixedUpdate()
        {
            state.Update(Time.fixedDeltaTime);
            if (state.IsAlive) return;

            Kill();
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void OnTriggerEnter(Collider collider)
        {
            HandleCollision(collider.gameObject);
        }

        private void HandleCollision(GameObject collidingObject)
        {
            bool targetHasHitPoints = collidingObject.TryGetComponent(out HitPoints targetHitPoints);
            if (targetHasHitPoints)
            {
                if (collidingObject.CompareTag("HoloShield"))
                    targetHitPoints.Points.Decrease(HandEvaluation.Score);
                else targetHitPoints.Points.Decrement();
            }

            if (state.IsThrown) Kill();
        }
    }
}
