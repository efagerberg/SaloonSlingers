using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public static class Fader
    {
        public static IEnumerator FadeTo(Image image, float targetAlpha, float duration)
        {
            FadeConfig config = new()
            {
                Start = image.color.a,
                End = targetAlpha,
                Duration = duration,
                GetDeltaTime = () => Time.deltaTime,
            };
            foreach (var alpha in AnimationCalculator.CalculateFade(config))
            {
                var newColor = image.color;
                newColor.a = alpha;
                image.color = newColor;
                yield return null;
            }
        }
    }
}
