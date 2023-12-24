using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Core;
using SaloonSlingers.Unity;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckDead : Conditional
    {
        public IReadOnlyPoints HitPoints { get; set; }

        public override void OnAwake()
        {
            HitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
        }

        public override TaskStatus OnUpdate()
        {
            return HitPoints.Value == 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
