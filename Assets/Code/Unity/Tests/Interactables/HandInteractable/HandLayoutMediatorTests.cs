using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.Interactables.Tests
{
    public class HandLayoutMediatorTests
    {
        public class TestDispose
        {
            [Test]
            public void Test_DespawnsTangibleCards_AndRemovesCardsFromSlingerAttributes()
            {
                RectTransform handPanelTransform = CreateComponent<RectTransform>("HandPanel");
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                IList<ITangibleCard> tangibleCards = new List<ITangibleCard>();
                HandLayoutMediator subject = new(handPanelTransform, canvasTransform, tangibleCards);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };

                (Func<Card, ITangibleCard> spawner,
                 IList<ITangibleCard> expectedDespawned) = GetSpawnerWithExpectedSpawned();

                List<ITangibleCard> actualDespawned = new();
                void cardDespawner(ITangibleCard t)
                {
                    actualDespawned.Add(t);
                    return;
                }
                subject.AddCardToHand(10, testAttributes, spawner, noRotationCalculator);
                subject.AddCardToHand(10, testAttributes, spawner, noRotationCalculator);
                subject.Dispose(cardDespawner, testAttributes.Hand);

                Assert.AreEqual(0, tangibleCards.Count());
                CollectionAssert.AreEquivalent(expectedDespawned, actualDespawned);
            }
        }

        public class TestAddCardToHand
        {
            [TestCase(1, 1)]
            [TestCase(2, 5)]
            [TestCase(1, 10)]
            [TestCase(10, 5)]
            public void Test_WhenCardsCanBeAdded_ReturnsExpectedTangibleCards(int n, int maxSize)
            {
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                IList<ITangibleCard> tangibleCards = new List<ITangibleCard>();
                HandLayoutMediator subject = new(panelTransform, canvasTransform, tangibleCards);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };
                (Func<Card, ITangibleCard> spawner,
                 IList<ITangibleCard> expectedTangibles) = GetSpawnerWithExpectedSpawned();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(15f);
                Enumerable.Range(0, n).ToList().ForEach(
                    _ => subject.AddCardToHand(maxSize, testAttributes, spawner, rotationCalculator)
                );

                AssertExpectedTangibleCards(
                    panelTransform,
                    expectedTangibles,
                    rotationCalculator,
                    testAttributes.Hand,
                    tangibleCards
                );
            }

            [Test]
            public void Test_WhenCommitted_ReturnsEmpty()
            {
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                IList<ITangibleCard> tangibleCards = new List<ITangibleCard>();
                HandLayoutMediator subject = new(panelTransform, canvasTransform, tangibleCards);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(15f);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };
                (Func<Card, ITangibleCard> spawner,
                 IList<ITangibleCard> expectedTangibles) = GetSpawnerWithExpectedSpawned();
                subject.AddCardToHand(100, testAttributes, spawner, rotationCalculator);
                subject.ToggleCommitHand(rotationCalculator, new InputAction.CallbackContext());
                subject.AddCardToHand(100, testAttributes, spawner, rotationCalculator);

                Assert.AreEqual(tangibleCards.Count(), 1);
                AssertExpectedTangibleCards(
                    panelTransform,
                    expectedTangibles,
                    rotationCalculator,
                    testAttributes.Hand,
                    tangibleCards
                );
            }

            private static void AssertExpectedTangibleCards(
                RectTransform expectedParent,
                IList<ITangibleCard> expectedTangibles,
                Func<int, IEnumerable<float>> rotationCalculator,
                IList<Card> actualHand,
                IList<ITangibleCard> actualTangibles
            )
            {
                CollectionAssert.AreEquivalent(expectedTangibles, actualTangibles);
                CollectionAssert.AreEquivalent(
                    new List<Transform> { expectedParent },
                    actualTangibles.Select(x => x.transform.parent).Distinct()
                );
                CollectionAssert.AreEquivalent(
                    actualHand,
                    actualTangibles.Select(x => x.Card)
                );
                Assert.That(
                    rotationCalculator(expectedTangibles.Count()),
                    Is.EqualTo(actualTangibles.Select(x => x.transform.localEulerAngles.z)).Within(0.001f)
                );
            }
        }

        public class TestToggleCommitHand
        {
            [Test]
            public void Test_WhenNoCardAddedAndCommitted_Returns0WidthCanvasSizeDelta_AndEmptyList()
            {
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                IList<ITangibleCard> tangibleCards = new List<ITangibleCard>();
                HandLayoutMediator subject = new(panelTransform, canvasTransform, tangibleCards);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(-10f);
                subject.ToggleCommitHand(rotationCalculator, new InputAction.CallbackContext());

                AssertExpectedCommittedResult(canvasTransform.sizeDelta.x, tangibleCards, 0, 0);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenCardsAdded_ThenHandCommitted_ReturnsCardWidthCanvasSizeDelta_AndListWithNoRotation(int n)
            {
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                IList<ITangibleCard> tangibleCards = new List<ITangibleCard>();
                HandLayoutMediator subject = new(panelTransform, canvasTransform, tangibleCards);
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToHand(n, testAttributes, spawner, rotationCalculator);
                });
                subject.ToggleCommitHand(rotationCalculator, new InputAction.CallbackContext());

                AssertExpectedCommittedResult(canvasTransform.sizeDelta.x, tangibleCards, tangibleCards.First().GetComponent<RectTransform>().rect.width, n);
            }

            [TestCase(1)]
            [TestCase(3)]
            [TestCase(5)]
            public void Test_WhenCardsAdded_ThenHandCommitted_AndUncommitted_ReturnsOriginalWidthCanvasSizeDelta_AndListWithRotation(int n)
            {
                RectTransform canvasTransform = CreateComponent<RectTransform>("HandCanvas");
                RectTransform panelTransform = CreateComponent<RectTransform>("HandPanel");
                IList<ITangibleCard> tangibleCards = new List<ITangibleCard>();
                HandLayoutMediator subject = new(panelTransform, canvasTransform, tangibleCards);
                PlayerAttributes testAttributes = new()
                {
                    Deck = new Deck(),
                    Hand = new List<Card>()
                };
                (var spawner, var spawned) = GetSpawnerWithExpectedSpawned();
                Func<int, IEnumerable<float>> rotationCalculator = SimpleRotationCalculatorFactory(10f);
                Enumerable.Range(0, n).ToList().ForEach(_ =>
                {
                    subject.AddCardToHand(n, testAttributes, spawner, rotationCalculator);
                });
                subject.ToggleCommitHand(rotationCalculator, new InputAction.CallbackContext());
                subject.ToggleCommitHand(rotationCalculator, new InputAction.CallbackContext());

                AssertExpectedResult(canvasTransform.sizeDelta.x, tangibleCards, canvasTransform.rect.width, n, rotationCalculator(n));
            }

            private static void AssertExpectedResult(float actualWidthDelta, IList<ITangibleCard> cards, float expectedWidthDelta, int expectedCount, IEnumerable<float> expectedRotations)
            {
                Assert.AreEqual(actualWidthDelta, expectedWidthDelta);
                Assert.AreEqual(cards.Count(), expectedCount);
                Assert.That(
                    cards.Select(x => x.transform.localEulerAngles.z),
                    Is.EqualTo(expectedRotations).Within(0.001f)
                );
            }

            private static void AssertExpectedCommittedResult(float actualWidthDelta, IList<ITangibleCard> cards, float expectedWidthDelta, int expectedCount)
            {
                AssertExpectedResult(
                    actualWidthDelta,
                    cards,
                    expectedWidthDelta,
                    expectedCount,
                    Enumerable.Range(0, expectedCount).Select(_ => 0f)
                );
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

        private class TestTangibleCard : MonoBehaviour, ITangibleCard
        {
            private Card card;
            public Card Card { get => card; set => card = value; }
        }

        private static (Func<Card, ITangibleCard> spawner, IList<ITangibleCard> spawned) GetSpawnerWithExpectedSpawned()
        {
            List<ITangibleCard> spawned = new();
            ITangibleCard cardSpawner(Card c)
            {
                ITangibleCard t = CreateComponent<TestTangibleCard>();
                t.gameObject.AddComponent<RectTransform>();
                t.Card = c;
                spawned.Add(t);
                return t;
            }
            return (cardSpawner, spawned);
        }
    }
}
