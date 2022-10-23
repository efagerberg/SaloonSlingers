namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct PlayerAttributes : ISlingerAttributes
    {
        public Deck Deck { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        public int Dashes { get; set; }
        public float DashSpeed { get; set; }
        public float DashCooldown { get; set; }
        public Handedness Handedness { get; set; }
    }
}
