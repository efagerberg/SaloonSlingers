namespace GambitSimulator.Core
{
    public class PlayerAttributes
    {
        public Deck Deck { get; private set; }

        public PlayerAttributes(int numberOfCards)
        {
            Deck = new Deck(numberOfCards).Shuffle();
        }
    }
}
