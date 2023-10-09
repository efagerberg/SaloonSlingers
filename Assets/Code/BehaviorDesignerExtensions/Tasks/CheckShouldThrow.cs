using BehaviorDesigner.Runtime.Tasks;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckShouldThrow : Conditional
    {
        public SharedEnemyHandInteractableController Controller;

        public override void OnStart()
        {
            if (Controller == null)
            {
                Debug.LogWarning("No controller set");
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (Controller.Value?.Cards != null && Controller.Value.Cards.Count >= 5) return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
