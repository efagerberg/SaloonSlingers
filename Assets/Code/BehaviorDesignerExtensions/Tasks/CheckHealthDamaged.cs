using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHealthDamaged : Conditional
    {
        public SharedHitPoints hitPoints;
        private uint currentPoints;

        public override void OnAwake()
        {
            currentPoints = hitPoints.Value.Points.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (currentPoints > hitPoints.Value.Points.Value)
            {
                currentPoints = hitPoints.Value.Points.Value;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
