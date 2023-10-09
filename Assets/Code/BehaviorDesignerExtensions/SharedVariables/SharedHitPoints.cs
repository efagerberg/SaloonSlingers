using BehaviorDesigner.Runtime;

using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class SharedHitPoints : SharedVariable<HitPoints>
    {
        public static implicit operator SharedHitPoints(HitPoints value)
        {
            return new SharedHitPoints { mValue = value };
        }
    }
}
