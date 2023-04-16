using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core
{
    public static class DamageApplicator
    {
        public static void DoDamage(IHandEvaluator handEvaluator, IList<Card> source, IEnumerable<(Points, IList<Card>)> targets)
        {
            uint sourceScore = handEvaluator.Evaluate(source).Score;
            bool targetFilter((Points, IList<Card>) target)
            {
                return target.Item1.Value > 0 && sourceScore > handEvaluator.Evaluate(target.Item2).Score;
            }

            targets.Where(targetFilter).ToList().ForEach(target => target.Item1.Value -= 1);
        }
    }
}
