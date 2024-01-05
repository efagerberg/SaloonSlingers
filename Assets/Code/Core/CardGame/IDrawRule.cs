using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public interface IDrawRule
    {
        public bool CanDraw(DrawContext ctx);
        public void DrawSideEffect(DrawContext ctx) { }
    }

    public struct DrawContext
    {
        public IList<Card> Hand;
        public IDictionary<AttributeType, Points> AttributeRegistry;
        public HandEvaluation Evaluation;
        public Deck Deck;
    }
}
