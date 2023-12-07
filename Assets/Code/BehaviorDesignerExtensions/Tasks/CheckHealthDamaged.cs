using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHealthDamaged : Conditional
    {
        public SharedHitPoints HitPoints;
        private uint currentPoints;

        public override void OnAwake()
        {
            currentPoints = HitPoints.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (currentPoints > HitPoints.Value)
            {
                currentPoints = HitPoints.Value;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
