using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class DrawCostRule : IDrawRule
    {
        private readonly uint[] costs;
        private readonly AttributeType[] requiredAttributes = new[] { AttributeType.Money, AttributeType.Pot };

        public DrawCostRule(uint[] costs)
        {
            this.costs = costs;
        }

        public bool CanDraw(DrawContext ctx)
        {
            if (!requiredAttributes.All(ctx.AttributeRegistry.ContainsKey)) return false;

            var money = ctx.AttributeRegistry[AttributeType.Money];
            var nextPrice = GetNextCost(ctx.Hand);
            return money >= nextPrice;
        }

        public void OnDraw(DrawContext ctx)
        {
            var money = ctx.AttributeRegistry[AttributeType.Money];
            var pot = ctx.AttributeRegistry[AttributeType.Pot];
            var nextPrice = GetNextCost(ctx.Hand);
            money.Decrease(nextPrice);
            pot.Increase(nextPrice);
        }

        private uint GetNextCost(IReadOnlyCollection<Card> hand)
        {
            int handMaxI = hand.Count() - 1;
            int costMaxI = costs.Count() - 1;
            int nextIndex = costMaxI > handMaxI + 1 ? handMaxI + 1 : costMaxI;
            return costs[nextIndex];
        }
    }
}
