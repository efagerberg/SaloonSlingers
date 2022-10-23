using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public delegate void DeckEmptyHandler(Deck sender, EventArgs e);
    public delegate void DeckRefilledHandler(Deck sender, EventArgs e);

    public class CardDrawnEvent: EventArgs
    {
        public readonly Card cardDrawn;
        internal CardDrawnEvent(Card c) : base() => cardDrawn = c;
    }
    public delegate void CardDrawnHandler(Deck sender, CardDrawnEvent e);

    [Serializable]
    public class Deck
    {
        public const int NUMBER_OF_CARDS_IN_STANDARD_DECK = 52;
        private Random random;
        public event DeckEmptyHandler OnDeckEmpty;
        public event DeckRefilledHandler OnDeckRefilled;
        public event CardDrawnHandler OnCardDrawn;

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

        public Card RemoveFromTop()
        {
            Card c = cards.Dequeue();
            if (cards.Count == 0) OnDeckEmpty?.Invoke(this, EventArgs.Empty);
            OnCardDrawn?.Invoke(this, new CardDrawnEvent(c));
            return c;
        }

        public IEnumerable<Card> RemoveFromTop(int amount)
        {
            for (int i = 0; i < amount; i++)
                yield return RemoveFromTop();
        }

        public void ReturnCard(Card card)
        {
            if (cards.Count == 0) OnDeckRefilled?.Invoke(this, EventArgs.Empty);
            cards.Enqueue(card);
        }

        public void ReturnCards(IEnumerable<Card> _cards)
        {
            foreach (Card card in _cards)
                cards.Enqueue(card);
        }
    }

}
