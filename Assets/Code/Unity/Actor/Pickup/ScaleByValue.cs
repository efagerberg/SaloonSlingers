using System.ComponentModel;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    [Description(@"Determines the scale of an object given a value and some denomination")]
    public class ScaleByValue : MonoBehaviour
    {
        [SerializeField]
        [Description("What value should be used to determine the scale factor")]
        private float denomination = 100;
        [SerializeField]
        [Description(@"Whether to scale in a discrete buckets based on denomination
                       instead of continous.")]
        private bool quantize = false;

        public void Scale(Transform t, uint value)
        {
            t.localScale = Vector3.one * ScaleByValueCalculator.Calculate(value, denomination, quantize);
        }
    }

    public static class ScaleByValueCalculator
    {
        public static float Calculate(float value, float denomination, bool quantize)
        {
            float ratio = value / denomination;
            return 1 + (quantize ? Mathf.FloorToInt(ratio) : ratio);
        }
    }
}
