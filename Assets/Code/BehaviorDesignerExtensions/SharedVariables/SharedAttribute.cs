using BehaviorDesigner.Runtime;

using SaloonSlingers.Core;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class SharedAttribute : SharedVariable<Attribute>
    {
        public static implicit operator SharedAttribute(Attribute value)
        {
            return new SharedAttribute { mValue = value };
        }
    }
}
