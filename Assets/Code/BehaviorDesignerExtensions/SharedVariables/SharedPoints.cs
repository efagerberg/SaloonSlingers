using BehaviorDesigner.Runtime;

using SaloonSlingers.Core;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class SharedPoints : SharedVariable<Points>
    {
        public static implicit operator SharedPoints(Points value)
        {
            return new SharedPoints { mValue = value };
        }
    }
}
