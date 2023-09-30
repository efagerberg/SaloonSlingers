using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class WarHandEvaluator : IHandEvaluator
    {
        public HandEvaluation Evaluate(IEnumerable<Card> hand)
        {
            uint score = (uint)(hand.Sum(c => (uint)c.Value));
            return new HandEvaluation(HandNames.NONE, score);
        }
    }
}
