using System.Collections.Generic;

namespace SaloonSlingers.Core.SlingerAttributes
{
    public struct EnemyAttributes : ISlingerAttributes
    {
        public Hand Hand { get; set; }
        public int MaxHandSize { get; set; }
        public int Health { get; set; }
    }
}
