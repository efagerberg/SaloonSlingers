using NUnit.Framework;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class GameObjectExtensionsTests
    {
        [Test]
        public void GetComponentInImmediateChildren_FindsComponentOnFirstChild()
        {
            var subject = new GameObject("Parent");
            var child1 = new GameObject("Child1");
            child1.transform.SetParent(subject.transform);
            var child2 = new GameObject("Child2");
            child2.transform.SetParent(subject.transform);

            var found = subject.GetComponentInImmediateChildren<Transform>();
            Assert.That(found, Is.EqualTo(child1.transform));
        }

        [Test]
        public void GetComponentInImmediateChildren_FailsToFindInGrandChildren()
        {
            var subject = new GameObject("Parent");
            var child = new GameObject("Child");
            child.transform.SetParent(subject.transform);
            var grandChild = new GameObject("GrandChild");
            grandChild.transform.SetParent(child.transform);
            grandChild.AddComponent<LineRenderer>();

            var found = subject.GetComponentInImmediateChildren<LineRenderer>();
            Assert.That(found, Is.Null);

        }
    }

    public class TransformExtensionsTests
    {
        [Test]
        public void GetAlignmentReturnsLowNumber_WhenObjectsAlmostAligned()
        {
            var a = new GameObject("GameObjectA");
            var b = new GameObject("GameObjectB");
            b.transform.position = new Vector3(0, 0.1f, 0);

            Assert.That(a.transform.GetAlignment(b.transform), Is.LessThan(0.1f));
        }

        [Test]
        public void GetAlignmentReturnsHighNumber_WhenObjectsBarelyAligned()
        {
            var a = new GameObject("GameObjectA");
            var b = new GameObject("GameObjectB");
            b.transform.position = new Vector3(0, 0, -3000);

            Assert.That(a.transform.GetAlignment(b.transform), Is.GreaterThan(0.9f));
        }

        [Test]
        public void GetAlignmentReturnsLowNumber_WhenVectorAlmostAligned()
        {
            var a = new GameObject("GameObjectA");
            var b = new Vector3(0, 0.1f, 0);

            Assert.That(a.transform.GetAlignment(b), Is.LessThan(0.1f));
        }

        [Test]
        public void GetAlignmentReturnsHighNumber_WhenVectorsBarelyAligned()
        {
            var a = new GameObject("GameObjectA");
            var b = new Vector3(0, 0, 3000);

            Assert.That(a.transform.GetAlignment(b), Is.GreaterThan(0.9f));
        }
    }
}
