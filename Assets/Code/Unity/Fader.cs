using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public static class Fader
    {
        public static IEnumerator Fade(Action<float> callback, float duration, float startAlpha = 1, float endAlpha = 0)
        {
            FadeConfig config = new()
            {
                Start = startAlpha,
                End = endAlpha,
                Duration = duration,
                GetDeltaTime = () => Time.deltaTime,
            };
            foreach (var alpha in AnimationCalculator.CalculateFade(config))
            {
                callback(alpha);
                yield return null;
            }
        }
    }
}
