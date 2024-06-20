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
        private CardHand cardHand;

        public override void OnStart()
        {
            absorber = Enemy.Value.GetComponent<HandAbsorber>();
            cardHand = Controller.Value.GetComponent<CardHand>();
        }

        public override TaskStatus OnUpdate()
        {
            if (absorber == null || cardHand == null || cardHand.Evaluation.Name == Core.HandNames.BUST)
                return TaskStatus.Failure;

            absorber.Absorb(Enemy.Value.ShieldHitPoints, cardHand);
            Controller.Value = null;
            return TaskStatus.Success;
        }
    }
}
