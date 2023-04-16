namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct PlayerAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }
        public Points HealthPoints { get; }
        public Dash Dash { get; }
        public PlayerAttributes(Deck deck, Points health, Dash dash)
        {
            Deck = deck;
            HealthPoints = health;
            Dash = dash;
        }
    }
}
