using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{

    public class DeathDetectorTests
    {
        [Test]
        public void ResetActor_EmitsResetUnityEvent()
        {
            var subject = TestUtils.CreateComponent<DeathDetector>();
            bool eventEmitted = false;
            subject.OnReset.AddListener(() => eventEmitted = true);
            subject.ResetActor();

            Assert.That(eventEmitted);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator EmitsKilledEvents_InCorrectOrder_WhenHealthDepleted()
        {
            var subject = TestUtils.CreateComponent<DeathDetector>();
            var attributes = subject.gameObject.AddComponent<Attributes>();
            var hp = new Core.Attribute(3);
            var eventsEmitted = new List<string>(2);
            attributes.Registry[AttributeType.Health] = hp;
            yield return null;
            subject.Killed += (object sender, EventArgs args) => eventsEmitted.Add("Killed");
            subject.OnKilled.AddListener(() => eventsEmitted.Add("OnKilled"));
            hp.Decrease(3);
            yield return null;
            var expected = new List<string> { "OnKilled", "Killed" };

            Assert.That(eventsEmitted, Is.EqualTo(expected));
        }
    }
}
