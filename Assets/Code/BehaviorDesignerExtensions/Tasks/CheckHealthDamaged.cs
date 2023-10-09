using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHealthDamaged : Conditional
    {
        public SharedHitPoints hitPoints;
        private uint startingPoints;

        public override void OnAwake()
        {
            startingPoints = hitPoints.Value.Points.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (startingPoints > hitPoints.Value.Points.Value)
            {
                startingPoints = hitPoints.Value.Points.Value;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
