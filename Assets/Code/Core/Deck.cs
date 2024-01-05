using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public delegate void DeckEmptyHandler(Deck sender, EventArgs e);
    public delegate void DeckRefilledHandler(Deck sender, EventArgs e);

    [Serializable]
    public class Deck
    {
        public const int NUMBER_OF_CARDS_IN_STANDARD_DECK = 52;
        private Random random;
        public event DeckEmptyHandler Emptied;
        public event DeckRefilledHandler Refilled;

        private readonly Queue<Card> cards = new();

        public Deck(int numberOfCards = NUMBER_OF_CARDS_IN_STANDARD_DECK)
        {
            var vals = Enum.GetValues(typeof(Values)).Cast<Values>();
            var suits = Enum.GetValues(typeof(Suits)).Cast<Suits>();
            while (numberOfCards > 0)
            {
                foreach (Suits suit in suits)
                {
                    foreach (Values val in vals)
                    {
                        if (numberOfCards <= 0) return;
                        cards.Enqueue(new Card(val, suit));
                        numberOfCards -= 1;
                    }
                }
            }
        }

        public int Count { get => cards.Count; }
        public bool HasCards { get => Count > 0; }

        public Deck Shuffle()
        {
            var queueAsList = cards.ToList();
            random = new Random((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
            for (int i = 0; i < queueAsList.Count; i++)
            {
                int num = random.Next(0, queueAsList.Count - 1);
                (queueAsList[i], queueAsList[num]) = (queueAsList[num], queueAsList[i]);
            }

            cards.Clear();
            foreach (Card card in queueAsList)
                cards.Enqueue(card);

            return this;
        }

        public Card Draw()
        {
            Card c = cards.Dequeue();
            if (cards.Count == 0) Emptied?.Invoke(this, EventArgs.Empty);
            return c;
        }

        public IEnumerable<Card> Draw(int amount)
        {
            for (int i = 0; i < amount && Count > 0; i++)
                yield return Draw();
        }

        public void Return(Card card)
        {
            if (cards.Count == 0) Refilled?.Invoke(this, EventArgs.Empty);
            cards.Enqueue(card);
        }

        public void Return(IEnumerable<Card> _cards)
        {
            foreach (Card card in _cards)
                cards.Enqueue(card);
        }
    }
}
