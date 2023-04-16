namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }

        public EnemyAttributes(Deck deck)
        {
            Deck = deck;
        }
    }
}
