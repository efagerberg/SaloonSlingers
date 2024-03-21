using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public struct HandProjectile

    {
        public IList<Card> Cards
        {
            get
            {
                cards ??= new List<Card>();
                return cards;
            }
        }
        public HandEvaluation HandEvaluation
        {
            get; private set;
        }

        private bool isThrown;
        private Deck deck;
        private IDictionary<AttributeType, Attribute> attributeRegistry;
        private DrawContext drawCtx;
        private readonly bool Drawn { get => cards.Count > 0; }
        private List<Card> cards;


        public readonly bool CheckShouldDie(GameObject collidingObject)
        {
            return (isThrown &&
                    collidingObject.layer != LayerMask.NameToLayer("Environment") &&
                    collidingObject.layer != LayerMask.NameToLayer("Hand"));
        }

        public Card? Pickup(CardGame game)
        {
            if (Drawn) return null;

            return TryDrawCard(game);
        }

        public Card? TryDrawCard(CardGame game)
        {
            drawCtx.Deck = deck;
            drawCtx.Evaluation = HandEvaluation;
            drawCtx.Hand = Cards;
            drawCtx.AttributeRegistry = attributeRegistry;
            Card? card = game.Draw(drawCtx);
            if (card == null) return null;

            Cards.Add(card.Value);
            HandEvaluation = game.Evaluate(Cards);
            return card;
        }

        public void Throw()
        {
            isThrown = true;
        }

        public void Assign(Deck newDeck, IDictionary<AttributeType, Attribute> newAttributeRegistry)
        {
            deck = newDeck;
            attributeRegistry = newAttributeRegistry;
        }

        public void ResetProjectile()
        {
            Cards.Clear();
            HandEvaluation = new(HandNames.NONE, 0);
            isThrown = false;
        }

        public void Pause()
        {
            isThrown = false;
        }
    }
}
