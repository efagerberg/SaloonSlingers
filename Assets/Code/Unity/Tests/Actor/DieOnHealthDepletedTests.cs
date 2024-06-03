using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class DieOnHealthDepletedTests
    {
        [UnityTest]
        public IEnumerator ResetActor_ResetsAttributes()
        {
            CreateSubject(out var subject, out var actor, out var attributes);
            yield return null;
            actor.ResetActor();

            Assert.That(attributes.Registry.Values.Select(a => a.Value),
                        Is.EqualTo(attributes.Registry.Values.Select(a => a.InitialValue)));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator EmitsKilledEvents_WhenHealthDepleted()
        {
            CreateSubject(out var subject, out var actor, out var attributes);
            yield return null;

            bool eventEmitted = false;
            actor.OnKilled.AddListener((Actor.Actor sender) => eventEmitted = true);
            attributes.Registry[AttributeType.Health].Decrease(3);
            yield return null;
            var expected = new List<string> { "OnKilled", "Killed" };

            Assert.That(eventEmitted);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator OnlyEmitsEventsWhenEnabled_BasedOnEnableDisable()
        {
            CreateSubject(out var subject, out var actor, out var attributes);
            yield return null;
            subject.enabled = false;

            var eventsEmitted = 0;
            actor.OnKilled.AddListener((Actor.Actor sender) => eventsEmitted++);
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

        private static void CreateSubject(out DieOnHealthDepleted subject,
                                          out Actor.Actor actor,
                                          out Attributes attributes)
        {
            actor = TestUtils.CreateComponent<Actor.Actor>();
            subject = actor.gameObject.AddComponent<DieOnHealthDepleted>();
            attributes = subject.gameObject.AddComponent<Attributes>();
            var hp = new Attribute(3);
            subject.runInEditMode = true;
            attributes.Registry[AttributeType.Health] = hp;
        }
    }
}
