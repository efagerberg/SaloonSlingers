using System;
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

    public class HandProjectile : MonoBehaviour
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
                UnityEvent<HandProjectile> eventToInvoke = mode switch
                {
                    HandProjectileMode.Damage => OnDamageMode,
                    HandProjectileMode.Curse => OnMarkMode,
                    _ => throw new NotImplementedException(),
                };
                eventToInvoke.Invoke(this);
            }
        }
        public UnityEvent<HandProjectile, ICardGraphic> OnDraw = new();
        public UnityEvent<HandProjectile> OnThrow = new();
        public UnityEvent<HandProjectile> OnPause = new();
        public UnityEvent<HandProjectile> OnPickup = new();
        public UnityEvent<HandProjectile> OnMarkMode = new();
        public UnityEvent<HandProjectile> OnDamageMode = new();
        public UnityEvent<Actor> OnReset => actor.OnReset;
        public UnityEvent<Actor> OnKilled => actor.OnKilled;

        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private HandCoordinator handCoordinator = new();
        private HandProjectileMode mode;
        private Actor actor;

        public void Pickup(Func<GameObject> spawnCard, ICardGame game)
        {
            var card = handCoordinator.Pickup(game);
            OnPickup.Invoke(this);
            if (card != null) Draw(spawnCard, card.Value);
        }

        public void TryDrawCard(Func<GameObject> spawnCard, ICardGame game)
        {
            var card = handCoordinator.TryDrawCard(game);
            if (card == null) return;

            Draw(spawnCard, card.Value);
        }

        public void Throw()
        {
            OnThrow.Invoke(this);
        }

        public void Throw(Vector3 offset)
        {
            OnThrow.Invoke(this);
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
        }

        public void Assign(Deck newDeck, IReadOnlyDictionary<AttributeType, Core.Attribute> newAttributeRegistry)
        {
            handCoordinator.Assign(newDeck, newAttributeRegistry);
        }

        public void InitialEvaluate(CardGame game)
        {
            handCoordinator.HandEvaluation = game.Evaluate(Cards);
        }

        public void ResetProjectile()
        {
            handCoordinator.Reset();
            rigidBody.gameObject.layer = LayerMask.NameToLayer("UnassignedProjectile");
            Mode = HandProjectileMode.Damage;
        }

        public void Pause()
        {
            OnPause.Invoke(this);
        }

        public void HandleCollision(GameObject contactingObject)
        {
            var antagonistLayer = LayerMask.LayerToName(gameObject.layer) switch
            {
                "PlayerProjectile" => LayerMask.NameToLayer("Enemy"),
                "EnemyProjectile" => LayerMask.NameToLayer("Player"),
                _ => -1,
            };
            var collidingWithAntagnoist = antagonistLayer != -1 && contactingObject.layer == antagonistLayer;
            var isSelfLethal = (
                collidingWithAntagnoist ||
                (!rigidBody.isKinematic &&
                 contactingObject.layer != LayerMask.NameToLayer("Environment") &&
                 contactingObject.layer != LayerMask.NameToLayer("Hand"))
            );
            if (!isSelfLethal) return;

            actor.Kill(delay: true);
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
            actor = GetComponent<Actor>();
        }

        private void Draw(Func<GameObject> spawnCard, Card card)
        {
            var spawned = spawnCard();
            var cardGraphic = spawned.GetComponent<ICardGraphic>();
            cardGraphic.Card = card;
            OnDraw.Invoke(this, cardGraphic);
            if (Cards.Count > 1)
                Mode = HandProjectileMode.Damage;
        }
    }
}
