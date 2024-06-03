using System.Collections;

using NUnit.Framework;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class ActorTests
    {
        [UnityTest]
        public IEnumerator EmitsEventNextFrame()
        {
            var subject = TestUtils.CreateComponent<Actor.Actor>();
            var killed = false;
            void killedHandler(Actor.Actor sender) => killed = true;
            subject.OnKilled.AddListener(killedHandler);
            subject.Kill();
            yield return null;

            Assert.That(killed);
        }
    }
}
