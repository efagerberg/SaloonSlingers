using System.Collections;

using NUnit.Framework;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class ActorKillTests
    {
        [Test]
        public void WhenDelayIsFalse_EmitsEventRightAway()
        {
            var subject = TestUtils.CreateComponent<Actor.Actor>();
            var killed = false;
            void killedHandler(Actor.Actor sender) => killed = true;
            subject.Killed.AddListener(killedHandler);
            subject.Kill(delay: false);

            Assert.That(killed);
        }

        [UnityTest]
        public IEnumerator WhenDelayIsTrue_EmitsEventNextFrame()
        {
            var subject = TestUtils.CreateComponent<Actor.Actor>();
            var killed = false;
            void killedHandler(Actor.Actor sender) => killed = true;
            subject.Killed.AddListener(killedHandler);
            subject.Kill(delay: true);
            Assert.That(killed, Is.False);
            yield return null;

            Assert.That(killed);
        }
    }

    public class ResetActorTests
    {
        [UnityTest]
        public IEnumerator EmitsResetEvent()
        {
            var subject = TestUtils.CreateComponent<Actor.Actor>();
            subject.runInEditMode = true;
            yield return null;
            var reset = false;
            void resetListener(Actor.Actor sender) => reset = true;
            subject.Reset.AddListener(resetListener);
            subject.ResetActor();

            Assert.That(reset);
        }
    }
}
