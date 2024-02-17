using System;
using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public struct FadeConfig
    {
        public float Start;
        public float End;
        public float Duration;
        public Func<float> GetDeltaTime;
    }
    public static class AnimationCalculator
    {
        public static IEnumerable<float> CalculateFade(FadeConfig config)
        {
            float elapsedTime = 0f;
            while (elapsedTime < config.Duration)
            {
                elapsedTime += config.GetDeltaTime();
                float t = System.Math.Clamp(elapsedTime / config.Duration, 0, 1);
                yield return Lerp(config.Start, config.End, t);
            }
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * System.Math.Clamp(t, 0, 1);
        }
    }
}
