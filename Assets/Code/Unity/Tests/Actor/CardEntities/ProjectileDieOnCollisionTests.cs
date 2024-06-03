using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class ProjectileDieOnCollisionTests
    {
        [UnityTest]
        public IEnumerator OnlyDies_WhenCollisionShouldBeLethal(
            [ValueSource(nameof(TestCases))]
            CollisionTestCase testCase
        )
        {
            var subject = BuildSubject();
            var actor = subject.GetComponent<Actor.Actor>();
            subject.gameObject.layer = LayerMask.NameToLayer(testCase.layer);
            yield return null;
            var rb = subject.GetComponent<Rigidbody>();
            rb.isKinematic = testCase.isKinematic;
            var collidingObject = new GameObject("CollidingObject")
            {
                layer = LayerMask.NameToLayer(testCase.collidingObjectLayer)
            };
            var killed = false;
            void killedHandler(Actor.Actor sender) => killed = true;
            actor.OnKilled.AddListener(killedHandler);
            subject.HandleCollision(collidingObject);
            yield return null;

            Assert.That(killed, Is.EqualTo(testCase.expected));
        }

        private static ProjectileDieOnCollision BuildSubject(int nCards = 10)
        {
            var rb = TestUtils.CreateComponent<Rigidbody>();
            rb.useGravity = false;
            var actor = rb.gameObject.AddComponent<Actor.Actor>();
            actor.runInEditMode = true;
            var subject = rb.gameObject.AddComponent<ProjectileDieOnCollision>();
            subject.runInEditMode = true;
            return subject;
        }

        private static IEnumerable TestCases()
        {
            yield return new CollisionTestCase { isKinematic = false, layer = "Default", collidingObjectLayer = "Environment", expected = false };
            yield return new CollisionTestCase { isKinematic = false, layer = "Default", collidingObjectLayer = "Hand", expected = false };
            yield return new CollisionTestCase { isKinematic = true, layer = "Default", collidingObjectLayer = "Default", expected = false };
            yield return new CollisionTestCase { isKinematic = false, layer = "Default", collidingObjectLayer = "Default", expected = true };
            yield return new CollisionTestCase { isKinematic = false, layer = "EnemyProjectile", collidingObjectLayer = "Player", expected = true };
            yield return new CollisionTestCase { isKinematic = false, layer = "PlayerProjectile", collidingObjectLayer = "Enemy", expected = true };
            yield return new CollisionTestCase { isKinematic = true, layer = "EnemyProjectile", collidingObjectLayer = "Player", expected = true };
            yield return new CollisionTestCase { isKinematic = true, layer = "PlayerProjectile", collidingObjectLayer = "Enemy", expected = true };
            yield return new CollisionTestCase { isKinematic = true, layer = "PlayerProjectile", collidingObjectLayer = "Player", expected = false };
            yield return new CollisionTestCase { isKinematic = true, layer = "EnemyProjectile", collidingObjectLayer = "Enemy", expected = false };
            yield return new CollisionTestCase { isKinematic = false, layer = "PlayerProjectile", collidingObjectLayer = "Player", expected = true };
            yield return new CollisionTestCase { isKinematic = false, layer = "EnemyProjectile", collidingObjectLayer = "Enemy", expected = true };
        }

        public struct CollisionTestCase
        {
            public bool isKinematic;
            public string collidingObjectLayer;
            public string layer;
            public bool expected;
        }
    }
}
