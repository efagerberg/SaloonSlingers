using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHasShieldHitPoints : Conditional
    {
        public SharedEnemy Enemy;

        public override TaskStatus OnUpdate()
        {
            return Enemy.Value.shieldHitPoints > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
