using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core.HandEvaluators;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Core
{
    public static class HealthSystem
    {
        public static void DoDamage(IHandEvaluator handEvaluator, IList<Card> source, IEnumerable<(ISlingerAttributes, IList<Card>)> targets)
        {
            uint sourceScore = handEvaluator.Evaluate(source).Score;
            bool targetFilter((ISlingerAttributes, IList<Card>) target)
            {
                return target.Item1.Health > 0 && sourceScore > handEvaluator.Evaluate(target.Item2).Score;
            }

            targets.Where(targetFilter).ToList().ForEach(target => target.Item1.Health -= 1);
        }
    }
}
