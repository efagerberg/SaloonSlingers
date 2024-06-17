using NUnit.Framework;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class CalculateLaunchAngleTests
    {
        [TestCaseSource(nameof(Cases))]
        public void ReturnsExpectedAngle(Vector3 source, Vector3 target, float speed, bool low, float expected)
        {
            Assert.That(
                ProjectileMotion.CalculateLaunchAngle(source, target, speed, low),
                Is.EqualTo(expected).Within(0.01f)
            );
        }

        public static object[] Cases =
        {
            // Same position source and target, direct or upright to hit
            new object[] { Vector3.zero, Vector3.zero, 10, true, 0f },
            new object[] { Vector3.zero, Vector3.zero, 10, false, 90f },
            // Origin source, linearly 10 unit away target
            new object[] { Vector3.zero, new Vector3(10, 0, 0), 10, true, 39.40f },
            new object[] { Vector3.zero, new Vector3(10, 0, 0), 10, false, 50.59f },
            // Source and Target with 3d distance from one another
            new object[] { new Vector3(1, 2, 3), new Vector3(4, 5, 6), 33.3f, true, 36.35f },
            new object[] { new Vector3(1, 2, 3), new Vector3(4, 5, 6), 33.3f, false, 88.90f },
            // Long distance, low velocity, not possible
            new object[] { new Vector3(-100, -200, -300), new Vector3(4000, 5000, 6000), 1, true, float.NaN},
            new object[] { new Vector3(-100, -200, -300), new Vector3(4000, 5000, 6000), 1, false, float.NaN},
        };
    }
}
