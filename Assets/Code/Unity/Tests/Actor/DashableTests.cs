using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class DashableTests
    {
        [UnityTest]
        public IEnumerator Invincible_WhenDashing()
        {
            var subject = TestUtils.CreateComponent<Dashable>();
            var metaData = new ActionMetaData()
            {
                Duration = 0.5f,
                Cooldown = 0,
                RecoveryPeriod = 0,
            };
            subject.Initialize(new Core.Attribute(1), metaData);
            subject.runInEditMode = true;
            subject.Dash((Vector3 x) => { }, Vector3.forward);
            yield return null;

            Assert.That(subject.gameObject.layer,
                        Is.EqualTo(LayerMask.NameToLayer("Invincible")));
        }

        [UnityTest]
        public IEnumerator NoLongerInvincible_WhenDashingFinished()
        {
            var subject = TestUtils.CreateComponent<Dashable>();
            var originalLayer = subject.gameObject.layer;
            var metaData = new ActionMetaData()
            {
                Duration = 0,
                Cooldown = 0,
                RecoveryPeriod = 0,
            };
            subject.Initialize(new Core.Attribute(1), metaData);
            subject.runInEditMode = true;
            subject.Dash((Vector3 x) => { }, Vector3.forward);
            yield return null;

            Assert.That(subject.gameObject.layer,
                        Is.EqualTo(originalLayer));
        }

        [UnityTest]
        public IEnumerator MovesForwardBySpeed_WhenDashing()
        {
            var subject = TestUtils.CreateComponent<Dashable>();
            var metaData = new ActionMetaData()
            {
                Duration = 0.5f,
                Cooldown = 0,
                RecoveryPeriod = 0,
            };
            subject.Initialize(new Core.Attribute(1), metaData);
            subject.runInEditMode = true;
            Vector3 velocity = Vector3.zero;
            subject.Dash((Vector3 x) => { velocity = x; }, Vector3.forward);
            yield return null;

            Assert.That(velocity,
                        Is.EqualTo(Vector3.forward * subject.Speed * Time.deltaTime));
        }
    }
}
