using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{

    public class DeathDetectorTests
    {
        [UnityTest]
        public IEnumerator ResetActor_EmitsResetUnityEventAndResetsAttributes()
        {
            CreateSubject(out var subject, out var attributes);
            yield return null;

            var eventsEmitted = new List<string>(2);
            bool eventEmitted = false;
            subject.OnReset.AddListener(() => eventEmitted = true);
            subject.ResetActor();

            Assert.That(eventEmitted);
            Assert.That(attributes.Registry.Values.Select(a => a.Value),
                        Is.EqualTo(attributes.Registry.Values.Select(a => a.InitialValue)));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator EmitsKilledEvents_InCorrectOrder_WhenHealthDepleted()
        {
            CreateSubject(out var subject, out var attributes);
            yield return null;

            var eventsEmitted = new List<string>(2);
            subject.Killed += (object sender, EventArgs args) => eventsEmitted.Add("Killed");
            subject.OnKilled.AddListener(() => eventsEmitted.Add("OnKilled"));
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;
            var expected = new List<string> { "OnKilled", "Killed" };

            Assert.That(eventsEmitted, Is.EqualTo(expected));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator OnlyEmitsEventsWhenEnabled_BasedOnEnableDisable()
        {
            CreateSubject(out var subject, out var attributes);
            yield return null;
            subject.enabled = false;

            var eventsEmitted = new List<string>(2);
            subject.Killed += (object sender, EventArgs args) => eventsEmitted.Add("Killed");
            subject.OnKilled.AddListener(() => eventsEmitted.Add("OnKilled"));
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;
            var expected = new List<string> { "OnKilled", "Killed" };

            Assert.That(eventsEmitted, Is.Empty);
            attributes.Registry[AttributeType.Health].Reset();
            subject.enabled = true;
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;

            Assert.That(eventsEmitted, Is.EqualTo(expected));
        }

        private static void CreateSubject(out DeathDetector subject, out Attributes attributes)
        {
            subject = TestUtils.CreateComponent<DeathDetector>();
            attributes = subject.gameObject.AddComponent<Attributes>();
            var hp = new Core.Attribute(3);
            subject.runInEditMode = true;
            attributes.Registry[AttributeType.Health] = hp;
        }
    }
}
