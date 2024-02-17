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
        [TestCaseSource(nameof(MissingAttributesCases))]
        public void DoesNotDrop_WhenMisingRequiredAttributes(IDictionary<AttributeType, Attribute> registry)
        {
            var spawnerMock = new Mock<ISpawner<GameObject>>(MockBehavior.Strict);
            var dropPosition = Vector3.zero;
            var layer = 0;
            PickupDropper.Drop(registry, spawnerMock.Object, layer, dropPosition);

            spawnerMock.Verify(s => s.Spawn(), Times.Never);
        }

        static IEnumerable<object[]> MissingAttributesCases()
        {
            return new[]
            {
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> { }
                },
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> {
                        { AttributeType.Health, new Attribute(1) }
                    }
                },
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> {
                        { AttributeType.Money, new Attribute(1) }
                    }
                },
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> {
                        { AttributeType.Pot, new Attribute(1) }
                    }
                },
            };
        }

        [Test]
        [TestCaseSource(nameof(AllAttributesCases))]
        public void Drop_WhenHasMoneyAndPot(IDictionary<AttributeType, Attribute> registry,
                                            Vector3 dropPosition,
                                            int layer,
                                            Vector3 expectedScale,
                                            int expectedValue)
        {
            var spawnerMock = new Mock<ISpawner<GameObject>>(MockBehavior.Strict);
            var spawned = new GameObject();
            var pickup = spawned.AddComponent<Pickup>();
            spawnerMock.Setup(s => s.Spawn()).Returns(spawned);
            PickupDropper.Drop(registry, spawnerMock.Object, layer, dropPosition);

            Assert.That(spawned.transform.position, Is.EqualTo(dropPosition));
            Assert.That(spawned.transform.localScale, Is.EqualTo(expectedScale));
            Assert.That(pickup.Value, Is.EqualTo(expectedValue));
        }

        static IEnumerable<object[]> AllAttributesCases()
        {
            return new[]
            {
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> {
                        { AttributeType.Pot, new Attribute(25) },
                        { AttributeType.Money, new Attribute(100) },
                    },
                    new Vector3(1, 2, 3),
                    1,
                    new Vector3(1.25f, 1.25f, 1.25f),
                    25
                },
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> {
                        { AttributeType.Pot, new Attribute(5) },
                        { AttributeType.Money, new Attribute(10) },
                    },
                    new Vector3(-10, 4, 3),
                    3,
                    new Vector3(1.5f, 1.5f, 1.5f),
                    5
                },
                new object[]
                {
                    new Dictionary<AttributeType, Attribute> {
                        { AttributeType.Pot, new Attribute(5) },
                        { AttributeType.Money, new Attribute(1) },
                    },
                    new Vector3(-19, -42, 11),
                    3,
                    new Vector3(6f, 6f, 6f),
                    5
                }
            };
        }
    }
}
