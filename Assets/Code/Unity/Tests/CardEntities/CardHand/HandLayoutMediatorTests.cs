using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests.CardEntities
{
    public class HandLayoutMediatorTests
    {
        public class TestDispose
        {
            [Test]
            public void Test_Resets_Transform()
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);

                (Func<Card, ICardGraphic> spawner,
                 IList<ICardGraphic> expectedSpawned) = GetSpawnerWithExpectedSpawned();

                List<ICardGraphic> actualDespawned = new();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(15);
                subject.AddCardToLayout(spawner(new Card("AC")), rotationCalculator);
                subject.AddCardToLayout(spawner(new Card("TC")), rotationCalculator);
                subject.Dispose();

                AssertCardTransformReset(expectedSpawned);
            }

            [Test]
            public void Test_Removes_From_HandPanel()
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);

                (Func<Card, ICardGraphic> spawner,
                 IList<ICardGraphic> expectedSpawned) = GetSpawnerWithExpectedSpawned();

                List<ICardGraphic> actualDespawned = new();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(15);
                subject.AddCardToLayout(spawner(new Card("AC")), rotationCalculator);
                subject.AddCardToLayout(spawner(new Card("TC")), rotationCalculator);
                subject.Dispose();

                CollectionAssert.IsEmpty(panelTransform.GetComponentsInChildren<ICardGraphic>());
            }

            private static void AssertCardTransformReset(IList<ICardGraphic> expectedSpawned)
            {
                Assert.AreEqual(
                    expectedSpawned.Select(c => c.transform.localRotation).ToHashSet(),
                    new HashSet<Quaternion> { Quaternion.identity }
                );
                Assert.That(expectedSpawned.Select(c => c.transform.localPosition.z).ToHashSet().Count > 1);
                CollectionAssert.AreEquivalent(
                    expectedSpawned.Select(c => c.transform.localPosition),
                    expectedSpawned.Select(c => c.transform.localPosition).OrderBy(pos => pos.z)
                );
                Assert.That(expectedSpawned.Select(c => c.transform.parent).All(p => p == null));
            }
        }

        public class TestAddCardToLayout
        {
            [TestCase(1)]
            [TestCase(2)]
            [TestCase(10)]
            public void Test_SpawnsExpectedIntoLayout(int n)
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);
                (Func<Card, ICardGraphic> spawner,
                 IList<ICardGraphic> expectedGraphics) = GetSpawnerWithExpectedSpawned();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(15f);
                Enumerable.Range(0, n).ToList().ForEach(_ => subject.AddCardToLayout(spawner(new Card("AH")), rotationCalculator));
                ICardGraphic[] actualSpawned = panelTransform.GetComponentsInChildren<ICardGraphic>();

                AssertHasExpectedSpawnedCardGraphics(
                    panelTransform,
                    expectedGraphics,
                    actualSpawned
                );
            }

            private static void AssertHasExpectedSpawnedCardGraphics(
                RectTransform expectedParent,
                IList<ICardGraphic> expectedGraphics,
                IList<ICardGraphic> actualGraphics
            )
            {
                CollectionAssert.AreEquivalent(expectedGraphics, actualGraphics);
                CollectionAssert.AreEquivalent(
                    new List<Transform> { expectedParent },
                    actualGraphics.Select(x => x.transform.parent).Distinct()
                );
            }
        }

        public class TestApplyLayout
        {
            [Test]
            public void Test_WhenNoCardAddedAndCommitted_Returns0WidthCanvasSizeDelta_AndEmptyList()
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(-10f);
                subject.ApplyLayout(true, rotationCalculator);

                ICardGraphic[] cardGraphics = panelTransform.GetComponentsInChildren<ICardGraphic>();
                AssertExpectedCommittedLayoutResult(panelTransform.sizeDelta.x, cardGraphics, 0, 0);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenCardsAdded_ThenHandCommitted_Returns0WidthAndRotation(int n)
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToLayout(spawner(new Card("3D")), rotationCalculator);
                });
                subject.ApplyLayout(true, rotationCalculator);

                ICardGraphic[] cardGraphics = panelTransform.GetComponentsInChildren<ICardGraphic>();
                AssertExpectedCommittedLayoutResult(panelTransform.sizeDelta.x, cardGraphics, 0, n);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenCardsAdded_ThenHandCommitted_AndUncommitted_ReturnsOriginalWidthCanvasSizeDelta_AndListWithRotation(int n)
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToLayout(spawner(new Card("AD")), rotationCalculator);
                });
                subject.ApplyLayout(true, rotationCalculator);
                subject.ApplyLayout(false, rotationCalculator);

                ICardGraphic[] cardGraphics = panelTransform.GetComponentsInChildren<ICardGraphic>();
                AssertExpectedLayoutResult(panelTransform.sizeDelta.x, cardGraphics, panelTransform.rect.width, n);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenNotCommitted_Idempotent(int n)
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                HandLayoutMediator subject = new(panelTransform);
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToLayout(spawner(new Card("AD")), rotationCalculator);
                });
                subject.ApplyLayout(false, rotationCalculator);
                var afterFirst = panelTransform.GetComponentsInChildren<ICardGraphic>().Select(x => x.transform);
                subject.ApplyLayout(false, rotationCalculator);
                var afterSecond = panelTransform.GetComponentsInChildren<ICardGraphic>().Select(x => x.transform);

                Assert.AreEqual(afterFirst, afterSecond);
            }

            private static void AssertExpectedLayoutResult(float actualWidthDelta, IList<ICardGraphic> cards, float expectedWidthDelta, int expectedCount)
            {
                Assert.AreEqual(actualWidthDelta, expectedWidthDelta);
                Assert.AreEqual(cards.Count(), expectedCount);
            }

            private static void AssertExpectedCommittedLayoutResult(float actualWidthDelta, IList<ICardGraphic> cards, float expectedWidthDelta, int expectedCount)
            {
                AssertExpectedLayoutResult(
                    actualWidthDelta,
                    cards,
                    expectedWidthDelta,
                    expectedCount
                );
                Assert.That(cards.Select(x => x.transform.localEulerAngles), Is.EqualTo(Enumerable.Repeat(Vector3.zero, expectedCount)));
            }
        }

        private static T CreateComponent<T>(string name = null) where T : Component
        {
            GameObject go = new();
            T comp = go.AddComponent<T>();
            if (name != null) comp.name = name;
            return comp;
        }

        private static Func<int, IEnumerable<float>> SimpleRotationCalculatorFactory(float step)
        {
            return (n) => Enumerable.Range(0, n).Select((x, i) => i * step);
        }

        private class TestCardGraphic : MonoBehaviour, ICardGraphic
        {
            private Card card;
            public Card Card { get => card; set => card = value; }
            public void SetGraphics(Card card) { }
        }

        private static (Func<Card, ICardGraphic> spawner, IList<ICardGraphic> spawned) GetSpawnerWithExpectedSpawned()
        {
            List<ICardGraphic> spawned = new();
            ICardGraphic cardSpawner(Card c)
            {
                ICardGraphic t = CreateComponent<TestCardGraphic>();
                t.gameObject.AddComponent<RectTransform>();
                t.Card = c;
                spawned.Add(t);
                return t;
            }
            return (cardSpawner, spawned);
        }
    }
}
