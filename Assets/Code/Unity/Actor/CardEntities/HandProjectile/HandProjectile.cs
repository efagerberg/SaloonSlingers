using System;
using System.Collections;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class HandProjectile : MonoBehaviour, IActor
    {
        public event EventHandler Killed;
        public IList<Card> Cards { get; private set; } = new List<Card>();
        public bool Drawn { get => Cards.Count > 0; }
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
        public UnityEvent<ICardGraphic> OnDraw;
        public UnityEvent OnThrow = new();
        public UnityEvent<GameObject> OnKill = new();
        public UnityEvent OnReset = new();
        public UnityEvent OnPause = new();
        public UnityEvent<HandProjectile> OnPickup = new();

        [SerializeField]
        private float lifespanInSeconds = 1f;
        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private HandProjectileState state;
        private Deck deck;
        private IDictionary<AttributeType, Core.Attribute> attributeRegistry;
        private GameManager gameManager;
        private HandEvaluation handEvaluation;
        private bool requiresEvaluation = false;
        private DrawContext drawCtx;

        public void Pickup(Func<GameObject> spawnCard)
        {
            bool stackedBefore = state.IsStacked;
            state = state.Reset();
            if (!Drawn) TryDrawCard(spawnCard);
            OnPickup.Invoke(this);
        }

        public void TryDrawCard(Func<GameObject> spawnCard)
        {
            drawCtx.Deck = deck;
            drawCtx.Evaluation = HandEvaluation;
            drawCtx.Hand = Cards;
            drawCtx.AttributeRegistry = attributeRegistry;
            Card? card = gameManager.Saloon.HouseGame.Draw(drawCtx);
            if (card == null) return;

            Cards.Add(card.Value);
            GameObject spawned = spawnCard();
            ICardGraphic cardGraphic = spawned.GetComponent<ICardGraphic>();
            cardGraphic.Card = card.Value;
            requiresEvaluation = true;
            OnDraw.Invoke(cardGraphic);
        }

        public void Throw()
        {
            state = state.Throw();
            OnThrow.Invoke();
        }

        public void Throw(Vector3 offset)
        {
            Throw();
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
        }

        public void Assign(Deck newDeck, IDictionary<AttributeType, Core.Attribute> newAttributeRegistry)
        {
            deck = newDeck;
            attributeRegistry = newAttributeRegistry;
        }

        public bool IsThrown { get => state.IsThrown; }

        public void ResetActor()
        {
            OnReset.Invoke();
            state = state.Reset();
            Cards.Clear();
            requiresEvaluation = true;
            gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
        }

        public void Kill()
        {
            OnKill.Invoke(gameObject);
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }

        public void Pause()
        {
            OnPause.Invoke();
            state = state.Pause();
        }

        private void OnEnable()
        {
            state = new(lifespanInSeconds);
        }

        private void Awake()
        {
            gameManager = GameManager.Instance;
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
        }

        private void FixedUpdate()
        {
            state.Update(Time.fixedDeltaTime);
            if (state.IsAlive) return;

            StartCoroutine(nameof(KillNextFrame));
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void OnTriggerEnter(Collider collider)
        {
            HandleCollision(collider.gameObject);
        }

        /// <summary>
        /// Defers killing entity until the next frame. Useful in cases of collision where you want
        /// the entities colliding with this object to do some process before removing the actor.
        /// </summary>
        private IEnumerator KillNextFrame()
        {
            yield return null;
            Kill();
        }

        private void HandleCollision(GameObject collidingObject)
        {
            if (!state.IsThrown || collidingObject.layer == LayerMask.NameToLayer("Environment"))
                return;

            if (state.IsThrown && collidingObject.layer != LayerMask.NameToLayer("Hand"))
                StartCoroutine(KillNextFrame());
        }
    }
}
