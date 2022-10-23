namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public Deck Deck { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        public Handedness Handedness { get; set; }
    }
}
