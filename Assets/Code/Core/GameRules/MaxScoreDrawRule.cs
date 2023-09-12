namespace SaloonSlingers.Core
{
    public class MaxScoreDrawRule : IDrawRule
    {
        private readonly uint maxScore;

        public MaxScoreDrawRule(uint maxScore)
        {
            this.maxScore = maxScore;
        }

        public bool CanDraw(DrawContext ctx)
        {
            return ctx.Deck.HasCards && ctx.Evaluation.Score <= maxScore;
        }
    }
}