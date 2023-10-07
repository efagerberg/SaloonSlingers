using BehaviorDesigner.Runtime.Tasks;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckShouldDraw : Conditional
    {
        private float secondsRemaining = 1;

        public override TaskStatus OnUpdate()
        {
            secondsRemaining -= Time.deltaTime;
            if (secondsRemaining <= 0)
            {
                secondsRemaining = 1;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
