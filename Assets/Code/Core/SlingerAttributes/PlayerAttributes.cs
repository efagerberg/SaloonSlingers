namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct PlayerAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }
        public PlayerAttributes(Deck deck)
        {
            Deck = deck;
        }
    }
}
