using BehaviorDesigner.Runtime;

using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class SharedEnemyHandInteractableController : SharedVariable<EnemyHandInteractableController>
    {
        public static implicit operator SharedEnemyHandInteractableController(EnemyHandInteractableController value)
        {
            return new SharedEnemyHandInteractableController { mValue = value };
        }
    }
}
