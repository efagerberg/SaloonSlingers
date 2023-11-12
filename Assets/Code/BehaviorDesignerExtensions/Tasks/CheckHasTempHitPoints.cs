using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHasTempHitPoints : Conditional
    {
        public SharedEnemy Enemy;

        public override TaskStatus OnUpdate()
        {
            return Enemy.Value.shieldHitPoints.Points.Value > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
