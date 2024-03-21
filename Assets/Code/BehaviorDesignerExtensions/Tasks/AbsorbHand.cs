using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class AbsorbHand : Action
    {
        public SharedEnemy Enemy;
        public SharedEnemyHandInteractableController Controller;

        private HandAbsorber absorber;
        private HandProjectile projectile;

        public override void OnStart()
        {
            absorber = Enemy.Value.GetComponent<HandAbsorber>();
            if (Controller.Value == null) return;
            projectile = Controller.Value.GetComponent<HandProjectile>();
        }

        public override TaskStatus OnUpdate()
        {
            if (absorber == null || projectile == null) return TaskStatus.Failure;

            absorber.Absorb(Enemy.Value.ShieldHitPoints, projectile);
            Controller.Value = null;
            return TaskStatus.Success;
        }
    }
}
