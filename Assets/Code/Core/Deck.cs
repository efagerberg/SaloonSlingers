using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class Deck : Queue<Card>
    {
        public const int NUMBER_OF_CARDS_IN_STANDARD_DECK = 52;
        private Random random;

        public Deck(int numberOfCards = NUMBER_OF_CARDS_IN_STANDARD_DECK)
        {
            var vals = Enum.GetValues(typeof(Values)).Cast<Values>();
            var suits = Enum.GetValues(typeof(Suits)).Cast<Suits>();
            while (numberOfCards > 0)
            {
                foreach (var suit in suits)
                {
                    foreach (var val in vals)
                    {
                        if (numberOfCards <= 0) return;
                        Enqueue(new Card(suit, val));
                        numberOfCards -= 1;
                    }
                }
            }
        }

        public Deck Shuffle()
        {
            var queueAsList = this.ToList();
            random = new Random((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
            for (var i = 0; i < queueAsList.Count; i++)
            {
                var num = random.Next(0, queueAsList.Count - 1);
                var c = queueAsList[num];
                queueAsList[num] = queueAsList[i];
                queueAsList[i] = c;
            }

            Clear();
            foreach (var card in queueAsList)
                Enqueue(card);

            return this;
        }

        public Card RemoveFromTop()
        {
            return Dequeue();
        }

        public IEnumerable<Card> RemoveFromTop(int _amount)
        {
            for (var i = 0; i < _amount; i++)
                yield return RemoveFromTop();
        }

        public void ReturnCard(Card _card)
        {
            Enqueue(_card);
        }

        public void ReturnCards(IEnumerable<Card> _cards)
        {
            foreach (var card in _cards)
                Enqueue(card);
        }
    }

}
