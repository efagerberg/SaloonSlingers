using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using UnityEngine;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.CardEntities.Tests
{
    public class HandLayoutMediatorTests
    {
        public class TestDispose
        {
            [Test]
            public void Test_DespawnsCardGraphics_LeavingHandPanelEmpty()
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                CardHandLayoutMediator subject = new(panelTransform, canvasTransform);

                (Func<Card, ICardGraphic> spawner,
                 IList<ICardGraphic> expectedDespawned) = GetSpawnerWithExpectedSpawned();

                List<ICardGraphic> actualDespawned = new();
                void cardDespawner(ICardGraphic t)
                {
                    actualDespawned.Add(t);
                    return;
                }
                subject.AddCardToLayout(spawner(new Card("AC")), noRotationCalculator);
                subject.AddCardToLayout(spawner(new Card("TC")), noRotationCalculator);
                subject.Dispose(cardDespawner);

                AssertDespawned(panelTransform, expectedDespawned, actualDespawned);
            }

            private static void AssertDespawned(RectTransform panelTransform, IList<ICardGraphic> expectedDespawned, List<ICardGraphic> actualDespawned)
            {
                CollectionAssert.AreEquivalent(expectedDespawned, actualDespawned);
                ICardGraphic[] cardsInPanel = panelTransform.GetComponentsInChildren<ICardGraphic>();
                CollectionAssert.IsEmpty(cardsInPanel);
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
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                CardHandLayoutMediator subject = new(panelTransform, canvasTransform);
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
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                CardHandLayoutMediator subject = new(panelTransform, canvasTransform);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(-10f);
                subject.ApplyLayout(true, rotationCalculator);

                ICardGraphic[] cardGraphics = panelTransform.GetComponentsInChildren<ICardGraphic>();
                AssertExpectedCommittedLayoutResult(canvasTransform.sizeDelta.x, cardGraphics, 0, 0);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenCardsAdded_ThenHandCommitted_ReturnsCardWidthCanvasSizeDelta_AndListWithNoRotation(int n)
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                CardHandLayoutMediator subject = new(panelTransform, canvasTransform);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToLayout(spawner(new Card("3D")), rotationCalculator);
                });
                subject.ApplyLayout(true, rotationCalculator);

                ICardGraphic[] cardGraphics = panelTransform.GetComponentsInChildren<ICardGraphic>();
                AssertExpectedCommittedLayoutResult(canvasTransform.sizeDelta.x, cardGraphics, cardGraphics.First().GetComponent<RectTransform>().rect.width, n);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenCardsAdded_ThenHandCommitted_AndUncommitted_ReturnsOriginalWidthCanvasSizeDelta_AndListWithRotation(int n)
            {
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                CardHandLayoutMediator subject = new(panelTransform, canvasTransform);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToLayout(spawner(new Card("AD")), rotationCalculator);
                });
                subject.ApplyLayout(true, rotationCalculator);
                subject.ApplyLayout(false, rotationCalculator);

                ICardGraphic[] cardGraphics = panelTransform.GetComponentsInChildren<ICardGraphic>();
                AssertExpectedLayoutResult(canvasTransform.sizeDelta.x, cardGraphics, canvasTransform.rect.width, n);
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

        private static readonly Func<int, IEnumerable<float>> noRotationCalculator = SimpleRotationCalculatorFactory(0);

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
