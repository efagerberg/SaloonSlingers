using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public interface IDrawRule
    {
        public bool CanDraw(DrawContext ctx);
        public void OnDraw(DrawContext ctx) { }
    }

    public struct DrawContext
    {
        public IReadOnlyCollection<Card> Hand;
        public IReadOnlyDictionary<AttributeType, Attribute> AttributeRegistry;
        public HandEvaluation Evaluation;
        public Deck Deck;
    }
}
