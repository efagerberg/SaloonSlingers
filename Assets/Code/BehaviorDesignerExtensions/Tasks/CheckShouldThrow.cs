using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Core;
using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Actor;

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

    public class CheckCanDraw : Conditional
    {
        public SharedEnemy Enemy;
        public SharedEnemyHandInteractableController Controller;

        private HandProjectile projectile;

        public override void OnStart()
        {
            if (Controller.Value != null)
                projectile = Controller.Value.GetComponent<HandProjectile>();
        }

        public override TaskStatus OnUpdate()
        {
            if (Controller.Value == null || projectile == null) return TaskStatus.Success;

            var ctx = new DrawContext()
            {
                Hand = projectile.Cards,
                Evaluation = projectile.HandEvaluation,
                Deck = Enemy.Value.Deck
            };
            return GameManager.Instance.Saloon.HouseGame.CanDraw(ctx) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
