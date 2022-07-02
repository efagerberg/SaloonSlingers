using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.Slingers
{
    public interface ISlinger
    {
        public ISlingerAttributes Attributes { get; }
    }
}
