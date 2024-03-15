using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class PickupDropperTests
    {
        [Test]
        [TestCaseSource(nameof(DropCases))]
        public void Drop_WhenHasMoneyAndPot(Attribute attribute,
                                            Vector3 dropPosition,
                                            int layer)
        {
            var spawnerMock = new Mock<ISpawner<GameObject>>(MockBehavior.Strict);
            var spawned = new GameObject();
            var pickup = spawned.AddComponent<TestPickup>();
            spawnerMock.Setup(s => s.Spawn()).Returns(spawned);
            PickupDropper.Drop(attribute, spawnerMock.Object, layer, dropPosition);

            Assert.That(spawned.transform.position, Is.EqualTo(dropPosition));
            Assert.That(pickup.Value, Is.EqualTo(attribute.Value));
        }

        static IEnumerable<object[]> DropCases()
        {
            return new[]
            {
                new object[]
                {
                    new Attribute(25),
                    new Vector3(1, 2, 3),
                    1
                },
                new object[]
                {
                    new Attribute(5),
                    new Vector3(-10, 4, 3),
                    3,
                },
                new object[]
                {
                    new Attribute(1),
                    new Vector3(-19, -42, 11),
                    3
                }
            };
        }

        class TestPickup : MonoBehaviour, IPickup
        {
            public uint Value { get; set; }
        }
    }
}
