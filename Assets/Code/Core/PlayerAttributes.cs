using System;

namespace SaloonSlingers.Core
{
    public class PlayerAttributes
    {
        public Deck Deck { get; private set; }
        public int Health { get; set; }

        public PlayerAttributes(int numberOfCards, int health)
        {
            Deck = new Deck(numberOfCards).Shuffle();
            Health = health;
        }
    }
}
