using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckDead : Conditional
    {
        public SharedHitPoints HitPoints;

        public override TaskStatus OnUpdate()
        {
            return HitPoints.Value == 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
