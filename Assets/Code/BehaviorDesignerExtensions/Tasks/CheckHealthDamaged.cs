using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Core;
using SaloonSlingers.Unity;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHealthDamaged : Conditional
    {
        public IReadOnlyPoints HitPoints { get; set; }
        private uint currentPoints;

        public override void OnAwake()
        {
            HitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
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
