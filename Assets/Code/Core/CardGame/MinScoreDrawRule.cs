namespace SaloonSlingers.Core
{
    public class MinScoreDrawRule : IDrawRule
    {
        private readonly uint minScore;

        public MinScoreDrawRule(uint maxScore)
        {
            this.minScore = maxScore;
        }

        public bool CanDraw(DrawContext ctx)
        {
            return ctx.Evaluation.Score >= minScore;
        }
    }
}
