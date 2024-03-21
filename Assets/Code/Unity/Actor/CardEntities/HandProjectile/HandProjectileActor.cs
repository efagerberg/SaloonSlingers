using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class HandProjectileActor : Actor
    {
        public IList<Card> Cards { get => handProjectile.Cards; }
        public HandEvaluation HandEvaluation { get => handProjectile.HandEvaluation; }
        public UnityEvent<GameObject, ICardGraphic> OnDraw;
        public UnityEvent<GameObject> OnThrow = new();
        public UnityEvent<GameObject> OnPause = new();
        public UnityEvent<GameObject> OnPickup = new();

        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private HandProjectile handProjectile = new();

        public void Pickup(Func<GameObject> spawnCard, CardGame game)
        {
            var card = handProjectile.Pickup(game);
            if (card != null) Draw(spawnCard, card.Value);
            OnPickup.Invoke(gameObject);
        }

        public void TryDrawCard(Func<GameObject> spawnCard, CardGame game)
        {
            var card = handProjectile.TryDrawCard(game);
            if (card == null) return;

            Draw(spawnCard, card.Value);
        }

        public void Throw()
        {
            handProjectile.Throw();
            OnThrow.Invoke(gameObject);
        }

        public void Throw(Vector3 offset)
        {
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
            handProjectile.Throw();
            OnThrow.Invoke(gameObject);
        }

        public void Assign(Deck newDeck, IDictionary<AttributeType, Core.Attribute> newAttributeRegistry)
        {
            handProjectile.Assign(newDeck, newAttributeRegistry);
        }

        public override void ResetActor()
        {
            handProjectile.ResetProjectile();
            rigidBody.gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
            OnReset.Invoke(gameObject);
        }

        public void Kill()
        {
            OnKilled.Invoke(gameObject);
        }

        public void Pause()
        {
            handProjectile.Pause();
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
            if (!handProjectile.CheckShouldDie(collidingObject))
                return;

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
