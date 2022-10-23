namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct PlayerAttributes : ISlingerAttributes
    {
        public Deck Deck { get; }
        public Health Health { get; }
        public Dash Dash { get; }
        public Handedness Handedness { get; set; }

        public PlayerAttributes(Deck deck, Health health, Dash dash, Handedness handedness)
        {
            Deck = deck;
            Health = health;
            Dash = dash;
            Handedness = handedness;
        }
    }
}
