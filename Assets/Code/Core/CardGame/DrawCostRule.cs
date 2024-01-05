using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class DrawCostRule : IDrawRule
    {
        private readonly uint[] costs;

        public DrawCostRule(uint[] costs)
        {
            this.costs = costs;
        }

        public bool CanDraw(DrawContext ctx)
        {
            bool hasMoneyAttribute = ctx.AttributeRegistry.ContainsKey(AttributeType.Money);
            if (!hasMoneyAttribute) return false;

            var money = ctx.AttributeRegistry[AttributeType.Money];
            var nextPrice = GetNextCost(ctx.Hand);
            return money >= nextPrice;
        }

        public void DrawSideEffect(DrawContext ctx)
        {
            var money = ctx.AttributeRegistry[AttributeType.Money];
            var nextPrice = GetNextCost(ctx.Hand);
            money.Decrease(nextPrice);
        }

        private uint GetNextCost(IList<Card> hand)
        {
            int handMaxI = hand.Count() - 1;
            int costMaxI = costs.Count() - 1;
            int nextIndex = costMaxI > handMaxI + 1 ? handMaxI + 1 : costMaxI;
            return costs[nextIndex];
        }
    }
}
