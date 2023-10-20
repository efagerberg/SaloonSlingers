using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public static class Fader
    {
        public static IEnumerator FadeTo(CanvasGroup group, float targetAlpha, float duration)
        {
            FadeConfig config = new()
            {
                Start = group.alpha,
                End = targetAlpha,
                Duration = duration,
                GetDeltaTime = () => Time.deltaTime,
            };
            foreach (var alpha in AnimationCalculator.CalculateFade(config))
            {
                group.alpha = alpha;
                yield return null;
            }
        }
    }
}
