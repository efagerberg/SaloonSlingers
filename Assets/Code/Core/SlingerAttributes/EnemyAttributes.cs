namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }
        public Points HealthPoints { get; }
        public Handedness Handedness { get; set; }

        public EnemyAttributes(Deck deck, Points health, Handedness handedness)
        {
            Deck = deck;
            HealthPoints = health;
            Handedness = handedness;
        }
    }
}
