using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
                    HandProjectileMode.Damage => SwitchedToDamageMode,
                    HandProjectileMode.Curse => SwitchedToCurseMode,
                    _ => throw new NotImplementedException(),
                };
                eventToInvoke.Invoke(this);
            }
        }
        [FormerlySerializedAs("OnDraw")]
        public UnityEvent<HandProjectile, ICardGraphic> Drawn = new();
        [FormerlySerializedAs("OnThrow")]
        public UnityEvent<HandProjectile> Thrown = new();
        [FormerlySerializedAs("OnPause")]
        public UnityEvent<HandProjectile> Paused = new();
        [FormerlySerializedAs("OnPickup")]
        public UnityEvent<HandProjectile> PickedUp = new();
        [FormerlySerializedAs("OnMarkMode")]
        public UnityEvent<HandProjectile> SwitchedToCurseMode = new();
        [FormerlySerializedAs("OnDamageMode")]
        public UnityEvent<HandProjectile> SwitchedToDamageMode = new();

        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;
        private HandCoordinator handCoordinator = new();
        private HandProjectileMode mode;

        public void Pickup(Func<GameObject> spawnCard, ICardGame game)
        {
            var card = handCoordinator.Pickup(game);
            PickedUp.Invoke(this);
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
            Thrown.Invoke(this);
        }

        public void Throw(Vector3 offset)
        {
            Thrown.Invoke(this);
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
            Paused.Invoke(this);
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
            Drawn.Invoke(this, cardGraphic);
            if (Cards.Count > 1)
                Mode = HandProjectileMode.Damage;
        }
    }
}
