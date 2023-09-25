using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public interface IHandEvaluator
    {
        public HandEvaluation Evaluate(IEnumerable<Card> hand);
    }
}
