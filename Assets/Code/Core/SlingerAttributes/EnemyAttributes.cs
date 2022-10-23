namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }
        public Health Health { get; }
        public Handedness Handedness { get; set; }

        public EnemyAttributes(Deck deck, Health health, Handedness handedness)
        {
            Deck = deck;
            Health = health;
            Handedness = handedness;
        }
    }
}
