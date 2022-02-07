using System.Collections.Generic;

namespace SaloonSlingers.Core.SlingerAttributes
{
    public interface ISlingerAttributes
    {
        public Hand Hand { get; set; }
        public int MaxHandSize { get; set; }
        public int Health { get; set; }
    }
}
