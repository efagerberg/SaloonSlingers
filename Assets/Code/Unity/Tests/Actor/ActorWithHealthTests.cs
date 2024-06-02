using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class ActorWithHealthTests
    {
        [UnityTest]
        public IEnumerator ResetActor_EmitsResetUnityEventAndResetsAttributes()
        {
            CreateSubject(out var subject, out var attributes);
            yield return null;

            var eventsEmitted = new List<string>(2);
            bool eventEmitted = false;
            subject.OnReset.AddListener((subject) => eventEmitted = true);
            subject.ResetActor();

            Assert.That(eventEmitted);
            Assert.That(attributes.Registry.Values.Select(a => a.Value),
                        Is.EqualTo(attributes.Registry.Values.Select(a => a.InitialValue)));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator EmitsKilledEvents_WhenHealthDepleted()
        {
            CreateSubject(out var subject, out var attributes);
            yield return null;

            bool eventEmitted = false;
            subject.OnKilled.AddListener((Actor.Actor sender) => eventEmitted = true);
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;
            var expected = new List<string> { "OnKilled", "Killed" };

            Assert.That(eventEmitted);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator OnlyEmitsEventsWhenEnabled_BasedOnEnableDisable()
        {
            CreateSubject(out var subject, out var attributes);
            yield return null;
            subject.enabled = false;

            var eventsEmitted = 0;
            subject.OnKilled.AddListener((Actor.Actor sender) => eventsEmitted++);
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;
            var expected = 1;

            Assert.That(eventsEmitted, Is.EqualTo(0));
            attributes.Registry[AttributeType.Health].Reset();
            subject.enabled = true;
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;

            Assert.That(eventsEmitted, Is.EqualTo(expected));
        }

        private static void CreateSubject(out ActorWithHealth subject, out Attributes attributes)
        {
            subject = TestUtils.CreateComponent<ActorWithHealth>();
            attributes = subject.gameObject.AddComponent<Attributes>();
            var hp = new Core.Attribute(3);
            subject.runInEditMode = true;
            attributes.Registry[AttributeType.Health] = hp;
        }
    }
}
