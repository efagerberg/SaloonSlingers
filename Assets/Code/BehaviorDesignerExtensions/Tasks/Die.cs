using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class Die : Action
    {
        public SharedEnemyHandInteractableController Controller;

        private Enemy enemy;

        public override void OnStart()
        {
            enemy = GetComponent<Enemy>();
        }

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
            enemy.Kill();
        }
    }
}
