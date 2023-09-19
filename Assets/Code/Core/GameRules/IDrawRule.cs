using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public interface IDrawRule
    {
        public bool CanDraw(DrawContext ctx);
    }

    public struct DrawContext
    {
        public IEnumerable<Card> Hand;
        public HandEvaluation Evaluation;
        public Deck Deck;
    }
}
