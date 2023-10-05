using System.Collections;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class HomableTests
    {
        [UnityTest]
        [RequiresPlayMode()]
        public IEnumerator MovesTowardsTarget_OverTime()
        {
            var projectile = TestUtils.CreateComponent<Homable>();
            var target = new GameObject("TestTarget");
            projectile.Target = target.gameObject.transform;
            projectile.Strength = 10f;
            target.transform.position = new Vector3(0, 0, 10f);
            var projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.velocity = new Vector3(1, 1, 1);
            var distanceBefore = (target.transform.position - projectile.transform.position).magnitude;
            var alignmentBefore = target.transform.GetAlignment(projectile.transform);

            yield return new WaitForFixedUpdate();

            var distanceAfter = (target.transform.position - projectile.transform.position).magnitude;
            var alignmentAfter = target.transform.GetAlignment(projectile.transform);

            Assert.Less(distanceAfter, distanceBefore);
            Assert.Less(alignmentAfter, alignmentBefore);
        }

        [UnityTest]
        [RequiresPlayMode()]
        public IEnumerator DoesNotHome_WhenNotSet()
        {
            var projectile = TestUtils.CreateComponent<Homable>();
            var target = new GameObject("TestTarget");
            projectile.Strength = 10f;
            target.transform.position = new Vector3(0, 0, 10f);
            var projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.velocity = new Vector3(1, 1, 1);
            var distanceBefore = (target.transform.position - projectile.transform.position).magnitude;
            var alignmentBefore = target.transform.GetAlignment(projectile.transform);

            yield return new WaitForFixedUpdate();

            var distanceAfter = (target.transform.position - projectile.transform.position).magnitude;
            var alignmentAfter = target.transform.GetAlignment(projectile.transform);

            Assert.Less(distanceAfter, distanceBefore);
            Assert.AreEqual(alignmentAfter, alignmentBefore, 0.001f);
        }
    }
}
