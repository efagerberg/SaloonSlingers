using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity
{
    public interface ISlinger
    {
        public ISlingerAttributes Attributes { get; }
    }
}
