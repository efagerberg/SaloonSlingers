using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core.HandEvaluators;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Core
{
    public static class HealthSystem
    {
        public static void DoDamage(IHandEvaluator handEvaluator, IList<Card> sourceHand, IEnumerable<ISlingerAttributes> targets)
        {
            uint sourceScore = handEvaluator.Evaluate(sourceHand).Score;
            bool targetFilter(ISlingerAttributes target)
            {
                return target.Health > 0 && sourceScore > handEvaluator.Evaluate(target.Hand).Score;
            }

            targets.Where(targetFilter).ToList().ForEach(target => target.Health -= 1);
        }
    }
}
