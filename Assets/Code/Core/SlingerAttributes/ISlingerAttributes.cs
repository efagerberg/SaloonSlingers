using System.Collections.Generic;

namespace SaloonSlingers.Core.SlingerAttributes
{
    public enum Handedness
    {
        LEFT, RIGHT
    }

    public interface ISlingerAttributes
    {
        public Deck Deck { get; set; }
        public Handedness Handedness { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
    }
}
