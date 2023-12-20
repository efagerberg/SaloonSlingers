using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHealthDamaged : Conditional
    {
        private HitPoints hitPoints;
        private uint currentPoints;

        public override void OnAwake()
        {
            if (hitPoints == null)
                hitPoints = GetComponent<HitPoints>();
            currentPoints = hitPoints;
        }

        public override TaskStatus OnUpdate()
        {
            if (currentPoints > hitPoints)
            {
                currentPoints = hitPoints;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
