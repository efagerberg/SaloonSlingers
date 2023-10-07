using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class SharedRenderer : SharedVariable<Renderer>
    {
        public static implicit operator SharedRenderer(Renderer value) { return new SharedRenderer { mValue = value }; }
    }

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
