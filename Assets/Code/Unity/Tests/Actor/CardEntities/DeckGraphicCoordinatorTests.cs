using Moq;

using NUnit.Framework;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor.Tests
{
    public class DeckGraphicCoordinatorTests
    {
        [Test]
        public void SpawnDeck_SpawnsACardPerCardInDeck()
        {
            var spawnerMock = new Mock<ISpawner<GameObject>>();
            spawnerMock.Setup(instance => instance.Spawn()).Returns(() => new GameObject("StubCard"));
            var coordinator = new DeckGraphicCoordinator();
            var parent = new GameObject().transform;
            var deck = new Deck(2);
            coordinator.SpawnDeck(deck, parent, spawnerMock.Object);

            Assert.That(deck.Count, Is.EqualTo(parent.childCount));
        }

        [Test]
        public void SpawnDeck_Idempotent()
        {
            var spawnerMock = new Mock<ISpawner<GameObject>>();
            spawnerMock.Setup(instance => instance.Spawn()).Returns(() => new GameObject("StubCard"));
            var coordinator = new DeckGraphicCoordinator();
            var parent = new GameObject().transform;
            var deck = new Deck(6);
            float zOffset = 0.5f;
            coordinator.SpawnDeck(deck, parent, spawnerMock.Object);
            var before = parent.childCount;
            var instance = coordinator.Pop();
            instance.transform.parent = null;
            coordinator.SpawnDeck(deck, parent, spawnerMock.Object, zOffset);

            Assert.That(before, Is.EqualTo(parent.childCount));
            AssertCorrectDistance(parent.transform, zOffset);
        }

        private void AssertCorrectDistance(Transform root, float expectedDistance)
        {
            for (int i = 0; i < root.childCount - 1; i++)
            {
                float distance = Vector3.Distance(root.GetChild(i).position, root.GetChild(i + 1).position);
                Assert.That(Mathf.Abs(distance - expectedDistance) > Mathf.Epsilon, "Unexpected distance");
                Vector3 direction = (root.GetChild(i + 1).position - root.GetChild(i).position).normalized;
                Vector3 newPosition = root.GetChild(i).position + (direction * expectedDistance);
                root.GetChild(i + 1).position = newPosition;
            }
        }

        [Test]
        public void GetTopCard_ReturnsTopOfStack()
        {
            var coordinator = new DeckGraphicCoordinator();
            var spawnerMock = new Mock<ISpawner<GameObject>>();
            int i = 0;
            spawnerMock.Setup(instance => instance.Spawn()).Returns(() =>
            {
                var instance = new GameObject($"StubCard{i}");
                i++;
                return instance;
            });
            var parent = new GameObject().transform;
            var deck = new Deck(2);
            coordinator.SpawnDeck(deck, parent, spawnerMock.Object);
            var result = coordinator.Peek(parent.gameObject);

            Assert.That(result.name, Is.EqualTo("StubCard1"));
        }
    }
}
