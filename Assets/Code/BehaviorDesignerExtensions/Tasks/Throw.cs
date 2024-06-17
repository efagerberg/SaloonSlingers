using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class Throw : Action
    {
        public Vector3 ThrowOffset = new(0, -0.25f, 0f);
        public (float Min, float Max) ThrowSpeedRange;
        public SharedGameObject Target;
        public SharedEnemyHandInteractableController Controller;

        private ControllerSwapper swapper;

        public override void OnStart()
        {
            swapper = Controller.Value.GetComponent<ControllerSwapper>();
        }

        public override TaskStatus OnUpdate()
        {
            if (Target == null)
            {
                Debug.LogWarning("Missing target to throw at");
                return TaskStatus.Failure;
            }

            return DoThrow();
        }

        private TaskStatus DoThrow()
        {
            if (Controller == null) return TaskStatus.Failure;

            Controller.Value.transform.SetParent(null, true);
            var throwSpeed = Random.Range(ThrowSpeedRange.Min, ThrowSpeedRange.Max);
            var launchAngle = ProjectileMotion.CalculateLaunchAngle(
                Controller.Value.transform.position,
                Target.Value.transform.position + ThrowOffset,
                throwSpeed,
                low: true
            );
            if (launchAngle == float.NaN) return TaskStatus.Failure;

            var originalRotation = Controller.Value.transform.localEulerAngles;
            Controller.Value.transform.localEulerAngles = new Vector3(
                // Negate angle to rotate counter clockwise
                -launchAngle,
                originalRotation.y,
                // Make sure to throw face down
                originalRotation.z + 180
            );
            Controller.Value.Throw(Controller.Value.transform.forward * throwSpeed);
            swapper.SetController(ControllerTypes.PLAYER);
            Controller.Value = null;
            return TaskStatus.Success;
        }
    }
}
