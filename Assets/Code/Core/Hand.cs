using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core
{
    public class Hand
    {
        public HandType HandType { get; private set; }
        public IEnumerable<Card> Cards { get; private set; }
        public IHandEvaluator HandEvaluator
        {
            get { return _handEvaluator; }
            set
            {
                _handEvaluator = value;
                HandType = HandEvaluator.Evaluate(Cards);
            }
        }
        private IHandEvaluator _handEvaluator;

        public Hand(IHandEvaluator handEvaluator)
        {
            Cards = new List<Card>();
            HandEvaluator = handEvaluator;
        }

        public Hand(IHandEvaluator handEvaluator, IEnumerable<Card> cards)
        {
            Cards = cards;
            HandEvaluator = handEvaluator;
        }

        public void Add(Card card)
        {
            Cards = Cards.Append(card);
            HandType = HandEvaluator.Evaluate(Cards);
        }

        public void Remove(int index)
        {
            Cards = Cards.Where((x, i) => i != index);
            HandType = HandEvaluator.Evaluate(Cards);
        }
    }
}
