using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{

    public class HandProjectile : Actor
    {
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
        public UnityEvent<GameObject, ICardGraphic> OnDraw;
        public UnityEvent<GameObject> OnThrow = new();
        public UnityEvent<GameObject> OnPause = new();
        public UnityEvent<GameObject> OnPickup = new();

        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private bool isThrown = false;
        private Deck deck;
        private IDictionary<AttributeType, Core.Attribute> attributeRegistry;
        private GameManager gameManager;
        private HandEvaluation handEvaluation;
        private bool requiresEvaluation = true;
        private DrawContext drawCtx;
        private bool Drawn { get => Cards.Count > 0; }

        public void Pickup(Func<GameObject> spawnCard)
        {
            if (!Drawn) TryDrawCard(spawnCard);
            OnPickup.Invoke(gameObject);
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
            OnDraw.Invoke(gameObject, cardGraphic);
        }

        public void Throw()
        {
            isThrown = true;
            OnThrow.Invoke(gameObject);
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

        public override void ResetActor()
        {
            OnReset.Invoke(gameObject);
            Cards.Clear();
            requiresEvaluation = false;
            isThrown = false;
            gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
        }

        public void Kill()
        {
            OnKilled.Invoke(gameObject);
        }

        public void Pause()
        {
            OnPause.Invoke(gameObject);
            isThrown = false;
        }

        private void Awake()
        {
            gameManager = GameManager.Instance;
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
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
            if (!isThrown || collidingObject.layer == LayerMask.NameToLayer("Environment"))
                return;

            if (isThrown && collidingObject.layer != LayerMask.NameToLayer("Hand"))
                Kill();
        }
    }
}
