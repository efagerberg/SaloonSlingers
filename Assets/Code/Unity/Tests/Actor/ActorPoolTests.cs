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
            Actor.Actor actor = TestUtils.CreateComponent<Actor.Actor>();
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
            Actor.Actor actor = TestUtils.CreateComponent<Actor.Actor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(3, prefab, root);
            subject.Get();
            var toDespawn = subject.Get().GetComponent<Actor.Actor>();
            toDespawn.Kill();

            Assert.That(subject.SpawnedActorCount, Is.EqualTo(1));
        }

        [Test]
        public void OnActorDeath_ReturnsToPool()
        {
            Actor.Actor actor = TestUtils.CreateComponent<Actor.Actor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(1, prefab, root);

            var instance = subject.Get();
            var actorInstance = instance.GetComponent<Actor.Actor>();
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
            Actor.Actor actor = TestUtils.CreateComponent<Actor.Actor>();
            var prefab = actor.gameObject;
            var root = new GameObject("PoolRoot").transform;
            ActorPool subject = new(1, prefab, root);

            var instance = subject.Get(false);
            Assert.False(instance.activeSelf);
            Assert.That(instance.transform.parent, Is.EqualTo(root));
        }
    }
}
