namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }
        public Points HealthPoints { get; }

        public EnemyAttributes(Deck deck, Points health)
        {
            Deck = deck;
            HealthPoints = health;
        }
    }
}
