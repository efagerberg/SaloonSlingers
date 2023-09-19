using System.Linq;

namespace SaloonSlingers.Core
{
    public class MaxHandSizeDrawRule : IDrawRule
    {
        private readonly int maxSize;

        public MaxHandSizeDrawRule(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public bool CanDraw(DrawContext ctx)
        {
            return ctx.Hand.Count() < maxSize;
        }
    }
}
