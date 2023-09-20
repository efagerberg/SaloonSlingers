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
            return ctx.Evaluation.Score < maxScore;
        }
    }
}
