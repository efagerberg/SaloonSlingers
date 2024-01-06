using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class Draw : Action
    {
        public SharedEnemy Enemy;
        public SharedTransform AttachTransform;
        public SharedEnemyHandInteractableController ReturnedObject;

        public override TaskStatus OnUpdate()
        {
            DoDraw();
            return TaskStatus.Success;
        }

        private void DoDraw()
        {
            if (ReturnedObject.Value == null && AttachTransform.Value != null)
            {
                GameObject clone = SpawnInteractable();
                ReturnedObject.Value = clone.GetComponent<EnemyHandInteractableController>();
                ReturnedObject.Value.transform.SetParent(AttachTransform.Value, false);
            }
            ReturnedObject.Value.Draw(Enemy.Value.Deck, Enemy.Value.AttributeRegistry, LevelManager.Instance.CardSpawner.Spawn);
            return;
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = LevelManager.Instance.HandInteractableSpawner.Spawn();
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.ENEMY);
            return spawned;
        }
    }
}
