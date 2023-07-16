using System.Collections.Generic;

namespace SaloonSlingers.Core.HandEvaluators
{
    public interface IHandEvaluator
    {
        public HandEvaluation Evaluate(IEnumerable<Card> hand);
    }
}
