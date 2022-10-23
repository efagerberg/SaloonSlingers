using SaloonSlingers.Core.SlingerAttributes;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Slingers
{
    public interface ISlinger
    {
        public ISlingerAttributes Attributes { get; }
    }
}
