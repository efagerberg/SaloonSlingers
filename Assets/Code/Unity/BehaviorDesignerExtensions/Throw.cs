using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class Throw : Action
    {
        public float ThrowSpeed = 20f;
        public Vector3 ThrowOffset = new(0, 0.25f, 0f);
        public SharedGameObject Target;
        public SharedEnemyHandInteractableController Controller;

        public override TaskStatus OnUpdate()
        {
            if (Target == null)
            {
                Debug.LogWarning("Missing target to throw at");
                return TaskStatus.Failure;
            }

            DoThrow();
            return TaskStatus.Success;
        }

        private void DoThrow()
        {
            if (Controller == null) return;
            Controller.Value.transform.SetParent(null, true);
            // Aim for more center mass
            Vector3 heightOffset = new(0, 0.25f, 0);
            Vector3 direction = (Target.Value.transform.position - transform.position - heightOffset).normalized;
            Controller.Value.Throw(direction * ThrowSpeed);
            ControllerSwapper swapper = Controller.Value.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            Controller.Value = null;
        }
    }
}
