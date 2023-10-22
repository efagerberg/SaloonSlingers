using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class Die : Action
    {
        public SharedEnemyHandInteractableController Controller;
        public SharedEnemy Enemy;

        public override TaskStatus OnUpdate()
        {
            DoDespawn();
            return TaskStatus.Success;
        }

        private void DoDespawn()
        {
            var projectile = Controller.Value?.GetComponent<HandProjectile>();
            if (projectile != null && !projectile.IsThrown)
            {
                projectile.Kill();
                Controller.Value = null;
            }
            Enemy.Value.Kill();
        }
    }
}
