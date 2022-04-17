using System.Collections.Generic;

namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public IList<Card> Hand { get; set; }
        public Deck Deck { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        public Handedness Handedness { get; set; }
    }
}
