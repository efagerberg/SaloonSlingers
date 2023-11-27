using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHealthDamaged : Conditional
    {
        public SharedHitPoints hitPoints;
        private uint currentPoints;

        public override void OnAwake()
        {
            currentPoints = hitPoints.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (currentPoints > hitPoints.Value)
            {
                currentPoints = hitPoints.Value;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
