using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class FaderTests
    {
        [UnityTest]
        public IEnumerator FadesTo_Value_OverTime()
        {
            var group = TestUtils.CreateComponent<CanvasGroup>();
            group.alpha = 0;
            float duration = 1f;
            yield return Fader.FadeTo(group, 1, duration);

            Assert.That(group.alpha, Is.EqualTo(1));
        }
    }
}
