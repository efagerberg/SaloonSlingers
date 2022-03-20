using System.Collections.Generic;

namespace SaloonSlingers.Core.SlingerAttributes
{
    public interface ISlingerAttributes
    {
        public IList<Card> Hand { get; set; }
        public Deck Deck { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
    }
}
