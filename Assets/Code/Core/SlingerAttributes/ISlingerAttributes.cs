namespace SaloonSlingers.Core.SlingerAttributes
{
    public enum Handedness
    {
        LEFT, RIGHT
    }

    public interface ISlingerAttributes
    {
        public Deck Deck { get; }
        public Handedness Handedness { get; set; }
        public Points HealthPoints { get; }
    }
}
