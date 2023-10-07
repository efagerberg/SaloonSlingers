using BehaviorDesigner.Runtime.Tasks;

namespace SaloonSlingers.Unity
{
    public class EvaluateHand : Conditional
    {
        public SharedEnemyHandInteractableController Controller;

        public override TaskStatus OnUpdate()
        {
            if (Controller != null && Controller.Value?.Cards != null && Controller.Value.Cards.Count >= 5) return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
