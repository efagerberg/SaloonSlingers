using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace SaloonSlingers.Unity.Tests
{
    public class ProjectileThrowTests
    {
        [Test]
        public void EmitsEvent()
        {
            var subject = ProjectileTestHelper.BuildProjectile();
            var thrown = false;
            void throwHandler(Projectile sender) => thrown = true;
            subject.Thrown.AddListener(throwHandler);
            subject.Throw();

            Assert.That(thrown);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator AddsOffsetForce_WhenOffsetSupplied()
        {
            var subject = ProjectileTestHelper.BuildProjectile();
            var rb = subject.GetComponent<Rigidbody>();
            yield return null;
            var offset = new Vector3(1, 2, 3);
            subject.Throw(offset);
            yield return new WaitForFixedUpdate();

            Assert.That(rb.velocity, Is.EqualTo(offset));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator AddsOffsetForceAndTorque_WhenSuppliedBoth()
        {
            var subject = ProjectileTestHelper.BuildProjectile();
            var rb = subject.GetComponent<Rigidbody>();
            yield return null;
            var offset = new Vector3(1, 2, 3);
            var torque = new Vector3(4, 5, 6);
            subject.Throw(offset, torque);
            yield return new WaitForFixedUpdate();

            Assert.That(rb.velocity, Is.EqualTo(offset));
            // Torque is a little less forgiving when comparing each float component
            var comparer = new Vector3EqualityComparer(0.01f);
            Assert.That(rb.angularVelocity, Is.EqualTo(torque).Using(comparer));
        }
    }

    public static class ProjectileTestHelper
    {
        public static Projectile BuildProjectile()
        {
            var rb = TestUtils.CreateComponent<Rigidbody>();
            rb.useGravity = false;
            var subject = rb.gameObject.AddComponent<Projectile>();
            subject.runInEditMode = true;
            return subject;
        }
    }
}
