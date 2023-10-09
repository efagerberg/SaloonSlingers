using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class SetRendererColor : Action
    {
        public SharedColor Color;
        public SharedRenderer Renderer;

        public override TaskStatus OnUpdate()
        {
            if (Renderer == null)
            {
                Debug.LogWarning("Renderer is null");
                return TaskStatus.Failure;
            }
            if (Color == null)
            {
                Debug.LogWarning("Color is null");
                return TaskStatus.Failure;
            }

            Renderer.Value.material.color = Color.Value;
            return TaskStatus.Success;
        }
    }
}
