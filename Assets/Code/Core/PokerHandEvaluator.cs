using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class PokerHandEvaluator : IHandEvaluator
    {

        public int Evaluate(IEnumerable<Card> hand)
        {
            return 0;
        }

        public int GetMaxHandValue() => 10;
        public int GetMinHandValue() => 0;
    }
}
