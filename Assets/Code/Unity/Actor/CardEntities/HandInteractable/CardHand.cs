using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SaloonSlingers.Unity.Actor
{
    public class CardHand : MonoBehaviour
    {
        public IReadOnlyCollection<Card> Cards { get => handCoordinator.Cards; }
        public HandEvaluation Evaluation { get => handCoordinator.HandEvaluation; }
        [FormerlySerializedAs("OnDraw")]
        public UnityEvent<CardHand, ICardGraphic> Drawn = new();
        [FormerlySerializedAs("OnPause")]
        public UnityEvent<CardHand> Paused = new();
        [FormerlySerializedAs("OnPickup")]
        public UnityEvent<CardHand> PickedUp = new();

        private HandCoordinator handCoordinator = new();

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
        }
    }
}
