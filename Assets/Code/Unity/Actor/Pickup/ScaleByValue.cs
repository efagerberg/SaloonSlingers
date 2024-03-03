using System.ComponentModel;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    [Description(@"Determines the scale of an object given some value and some denomination")]
    public class ScaleByValue : MonoBehaviour
    {
        [SerializeField]
        [Description("What value should be used to determine the scale factor")]
        private float denomination = 100;
        [SerializeField]
        [Description(@"Whether the scale factor must be an integer or not.
                       Useful when you don't want to scale contiuously, and instead
                       want to scale when the value reaches some multiple of the denomination.")]
        private bool intScaleFactorsOnly = false;

        public void Scale(float value)
        {
            transform.localScale = ScaleByValueCalculator.Calculate(value, denomination, intScaleFactorsOnly);
        }
    }

    public static class ScaleByValueCalculator
    {
        public static Vector3 Calculate(float value, float denomination, bool intScaleFactorsOnly)
        {
            float ratio = value / denomination;
            float scaleFactor = 1 + (intScaleFactorsOnly ? Mathf.FloorToInt(ratio) : ratio);
            return scaleFactor * Vector3.one;
        }
    }
}
