﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GambitSimulator.Core
{
    public class Deck : Queue<Card>
    {
        private Random random;

        public Deck(int numberOfDecks = 1)
        {
            for (int i = 0; i < numberOfDecks; i++)
            {
                var vals = Enum.GetValues(typeof(Values)).Cast<Values>();
                var suits = Enum.GetValues(typeof(Suits)).Cast<Suits>();
                foreach (var suit in suits)
                {
                    foreach (var val in vals)
                    {
                        Enqueue(new Card(suit, val));
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
            {
                Enqueue(card);
            }

            return this;
        }

        public Card RemoveFromTop()
        {
            return Dequeue();
        }

        public void RemoveFromTop(int _amount, IList<Card> _cards)
        {
            for (var i = 0; i < _amount; i++)
            {
                _cards.Add(RemoveFromTop());
            }
        }

        public void ReturnCard(Card _card)
        {
            Enqueue(_card);
            Shuffle();
        }

        public void ReturnCards(IEnumerable<Card> _cards)
        {
            foreach (var card in _cards)
            {
                Enqueue(card);
            }
            Shuffle();
        }
    }

}
