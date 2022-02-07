using System.Collections.Generic;

namespace SaloonSlingers.Core.HandEvaluators
{
    public interface IHandEvaluator
    {
        public HandType Evaluate(IEnumerable<Card> hand);
    }
}
