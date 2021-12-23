using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public interface IHandEvaluator
    {
        public int Evaluate(IEnumerable<Card> hand);
        public int GetMaxHandValue();
        public int GetMinHandValue();
    }
}
