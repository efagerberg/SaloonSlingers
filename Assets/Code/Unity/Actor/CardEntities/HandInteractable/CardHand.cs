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

    public class CardHand : MonoBehaviour
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
                UnityEvent<CardHand> eventToInvoke = mode switch
                {
                    HandProjectileMode.Damage => SwitchedToDamageMode,
                    HandProjectileMode.Curse => SwitchedToCurseMode,
                    _ => throw new NotImplementedException(),
                };
                eventToInvoke.Invoke(this);
            }
        }
        [FormerlySerializedAs("OnDraw")]
        public UnityEvent<CardHand, ICardGraphic> Drawn = new();
        [FormerlySerializedAs("OnPause")]
        public UnityEvent<CardHand> Paused = new();
        [FormerlySerializedAs("OnPickup")]
        public UnityEvent<CardHand> PickedUp = new();
        [FormerlySerializedAs("OnMarkMode")]
        public UnityEvent<CardHand> SwitchedToCurseMode = new();
        [FormerlySerializedAs("OnDamageMode")]
        public UnityEvent<CardHand> SwitchedToDamageMode = new();

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

        public void Assign(Deck newDeck, IReadOnlyDictionary<AttributeType, Core.Attribute> newAttributeRegistry)
        {
            handCoordinator.Assign(newDeck, newAttributeRegistry);
        }

        public void InitialEvaluate(CardGame game)
        {
            handCoordinator.HandEvaluation = game.Evaluate(Cards);
        }

        public void ResetHand()
        {
            handCoordinator.Reset();
            Mode = HandProjectileMode.Damage;
        }

        public void Pause()
        {
            Paused.Invoke(this);
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
