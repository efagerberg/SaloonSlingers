using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public interface IHandEvaluator
    {
        public uint Evaluate(IEnumerable<Card> hand);
    }
}
