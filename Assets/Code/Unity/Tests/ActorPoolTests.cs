using System;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class ActorPoolTests
    {
        [Test]
        public void Get_ReturnsActiveInstanceOfPrefabInRoot()
        {
            TestActor actor = TestUtils.CreateComponent<TestActor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(1, prefab, root);

            var instance = subject.Get();
            Assert.That(instance.activeSelf);
            Assert.That(instance.transform.parent, Is.EqualTo(root));
        }

        [Test]
        public void CountSpawned_EqualsNumberOfActiveActors()
        {
            TestActor actor = TestUtils.CreateComponent<TestActor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(3, prefab, root);
            subject.Get();
            var toDespawn = subject.Get().GetComponent<TestActor>();
            toDespawn.Kill();


            Assert.That(subject.CountSpanwed, Is.EqualTo(1));
        }

        [Test]
        public void OnActorDeath_ReturnsToPool()
        {
            TestActor actor = TestUtils.CreateComponent<TestActor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(1, prefab, root);

            var instance = subject.Get();
            var actorInstance = instance.GetComponent<TestActor>();
            actorInstance.transform.SetParent(null);
            actorInstance.Kill();

            Assert.False(instance.activeSelf);
            Assert.That(instance.transform.parent, Is.EqualTo(root));
            Assert.That(instance.transform.localPosition, Is.EqualTo(Vector3.zero));
            Assert.That(instance.transform.localRotation, Is.EqualTo(Quaternion.identity));
        }

        [Test]
        public void Get_WhenAskedForInactive_ReturnsInactiveInstanceOfPrefabInRoot()
        {
            TestActor actor = TestUtils.CreateComponent<TestActor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(1, prefab, root);

            var instance = subject.Get(false);
            Assert.False(instance.activeSelf);
            Assert.That(instance.transform.parent, Is.EqualTo(root));
        }
    }

    class TestActor : MonoBehaviour, IActor
    {
        public event EventHandler Death;

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }

        public void ResetActor()
        {
            return;
        }
    }
}
