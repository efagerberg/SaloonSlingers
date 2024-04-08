using System;
using System.Collections;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public enum HandProjectileMode
    {
        Damage = 0,
        Curse = 1
    }

    public class HandProjectile : Actor
    {
        public IReadOnlyCollection<Card> Cards { get => handCoordinator.Cards; }
        public HandEvaluation HandEvaluation { get => handCoordinator.HandEvaluation; }
        public HandProjectileMode Mode
        {
            get => mode;
            set
            {
                if (value == mode) return;

                var canCurse = value == HandProjectileMode.Curse && Cards.Count == 1;
                if (!canCurse && value == HandProjectileMode.Curse)
                    return;

                mode = value;
                UnityEvent<GameObject> eventToInvoke = mode switch
                {
                    HandProjectileMode.Damage => OnDamageMode,
                    HandProjectileMode.Curse => OnMarkMode,
                    _ => throw new NotImplementedException(),
                };
                eventToInvoke.Invoke(gameObject);
            }
        }
        public UnityEvent<GameObject, ICardGraphic> OnDraw = new();
        public UnityEvent<GameObject> OnThrow = new();
        public UnityEvent<GameObject> OnPause = new();
        public UnityEvent<GameObject> OnPickup = new();
        public UnityEvent<GameObject> OnMarkMode = new();
        public UnityEvent<GameObject> OnDamageMode = new();

        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private HandCoordinator handCoordinator = new();
        private HandProjectileMode mode;

        public void Pickup(Func<GameObject> spawnCard, CardGame game)
        {
            var card = handCoordinator.Pickup(game);
            OnPickup.Invoke(gameObject);
            if (card != null) Draw(spawnCard, card.Value);
        }

        public void TryDrawCard(Func<GameObject> spawnCard, CardGame game)
        {
            var card = handCoordinator.TryDrawCard(game);
            if (card == null) return;

            Draw(spawnCard, card.Value);
        }

        public void Throw()
        {
            OnThrow.Invoke(gameObject);
        }

        public void Throw(Vector3 offset)
        {
            OnThrow.Invoke(gameObject);
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
        }

        public void Assign(Deck newDeck, IDictionary<AttributeType, Core.Attribute> newAttributeRegistry)
        {
            handCoordinator.Assign(newDeck, newAttributeRegistry);
        }

        public override void ResetActor()
        {
            handCoordinator.Reset();
            rigidBody.gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
            Mode = HandProjectileMode.Damage;
            OnReset.Invoke(gameObject);
        }

        public void Kill()
        {
            StartCoroutine(nameof(KillNextFrame));
        }

        /// <summary>
        /// Sometimes we want other objects to react to being hit by a projectile.
        /// This requires the object to live for an extra frame.
        /// </summary>
        /// <returns></returns>
        private IEnumerator KillNextFrame()
        {
            yield return null;
            OnKilled.Invoke(gameObject);
        }

        public void Pause()
        {
            OnPause.Invoke(gameObject);
        }

        public void HandleCollision(GameObject contactingObject)
        {
            var isSelfLethal = (
                !rigidBody.isKinematic &&
                contactingObject.layer != LayerMask.NameToLayer("Environment") &&
                contactingObject.layer != LayerMask.NameToLayer("Hand")
            );
            if (!isSelfLethal) return;

            Kill();
        }

        public void HandleCollision(Collision other)
        {
            HandleCollision(other.gameObject);
        }

        public void HandleCollision(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
        }

        private void Draw(Func<GameObject> spawnCard, Card card)
        {
            var spawned = spawnCard();
            var cardGraphic = spawned.GetComponent<ICardGraphic>();
            cardGraphic.Card = card;
            OnDraw.Invoke(gameObject, cardGraphic);
            if (Cards.Count > 1)
                Mode = HandProjectileMode.Damage;
        }
    }
}
