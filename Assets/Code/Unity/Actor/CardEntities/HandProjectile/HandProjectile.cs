using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class HandProjectile : Actor
    {
        public IReadOnlyCollection<Card> Cards { get => harndCoordinator.Cards; }
        public HandEvaluation HandEvaluation { get => harndCoordinator.HandEvaluation; }
        public UnityEvent<GameObject, ICardGraphic> OnDraw = new();
        public UnityEvent<GameObject> OnThrow = new();
        public UnityEvent<GameObject> OnPause = new();
        public UnityEvent<GameObject> OnPickup = new();

        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private HandCoordinator harndCoordinator = new();

        public void Pickup(Func<GameObject> spawnCard, CardGame game)
        {
            var card = harndCoordinator.Pickup(game);
            if (card != null) Draw(spawnCard, card.Value);
            OnPickup.Invoke(gameObject);
        }

        public void TryDrawCard(Func<GameObject> spawnCard, CardGame game)
        {
            var card = harndCoordinator.TryDrawCard(game);
            if (card == null) return;

            Draw(spawnCard, card.Value);
        }

        public void Throw()
        {
            OnThrow.Invoke(gameObject);
        }

        public void Throw(Vector3 offset)
        {
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
            OnThrow.Invoke(gameObject);
        }

        public void Assign(Deck newDeck, IDictionary<AttributeType, Core.Attribute> newAttributeRegistry)
        {
            harndCoordinator.Assign(newDeck, newAttributeRegistry);
        }

        public override void ResetActor()
        {
            harndCoordinator.Reset();
            rigidBody.gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
            OnReset.Invoke(gameObject);
        }

        public void Kill()
        {
            OnKilled.Invoke(gameObject);
        }

        public void Pause()
        {
            OnPause.Invoke(gameObject);
        }

        private void Awake()
        {
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
            var isSelfLethal = (
                !rigidBody.isKinematic &&
                collidingObject.layer != LayerMask.NameToLayer("Environment") &&
                collidingObject.layer != LayerMask.NameToLayer("Hand")
            );
            if (!isSelfLethal) return;

            Kill();
        }

        private void Draw(Func<GameObject> spawnCard, Card card)
        {
            var spawned = spawnCard();
            var cardGraphic = spawned.GetComponent<ICardGraphic>();
            cardGraphic.Card = card;
            OnDraw.Invoke(gameObject, cardGraphic);
        }
    }
}
