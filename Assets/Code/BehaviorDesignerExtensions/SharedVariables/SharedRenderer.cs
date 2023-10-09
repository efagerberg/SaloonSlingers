using BehaviorDesigner.Runtime;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class SharedRenderer : SharedVariable<Renderer>
    {
        public static implicit operator SharedRenderer(Renderer value)
        {
            return new SharedRenderer { mValue = value };
        }
    }
}
