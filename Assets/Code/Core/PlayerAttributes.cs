using System;

namespace SaloonSlingers.Core
{
    public struct PlayerAttributes
    {
        public Deck Deck { get; set; }
        public int Health { get; set; }
        public int Dashes { get; set; }
        public float DashSpeed { get; set; }
        public float DashCooldown { get; set; }
    }
}
