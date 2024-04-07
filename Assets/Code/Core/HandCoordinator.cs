using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public struct HandCoordinator
    {
        public IReadOnlyCollection<Card> Cards
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

        private Deck deck;
        private IDictionary<AttributeType, Attribute> attributeRegistry;
        private DrawContext drawCtx;
        private bool Drawn { get => cards != null && cards.Count > 0; }
        private List<Card> cards;

        public Card? Pickup(CardGame game)
        {
            if (Drawn) return null;

            return TryDrawCard(game);
        }

        public Card? TryDrawCard(CardGame game)
        {
            cards ??= new List<Card>();
            drawCtx.Deck = deck;
            drawCtx.Evaluation = HandEvaluation;
            drawCtx.Hand = cards;
            drawCtx.AttributeRegistry = attributeRegistry;
            Card? card = game.Draw(drawCtx);
            if (card == null) return null;

            cards.Add(card.Value);
            HandEvaluation = game.Evaluate(Cards);
            return card;
        }

        public void Assign(Deck newDeck, IDictionary<AttributeType, Attribute> newAttributeRegistry)
        {
            deck = newDeck;
            attributeRegistry = newAttributeRegistry;
        }

        public void Reset()
        {
            cards?.Clear();
            HandEvaluation = HandEvaluation.EMPTY;
        }
    }
}
