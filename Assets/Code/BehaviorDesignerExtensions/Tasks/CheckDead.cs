using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckDead : Conditional
    {
        public SharedHitPoints hitPoints;

        public override TaskStatus OnUpdate()
        {
            return hitPoints.Value.Points.Value <= 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
