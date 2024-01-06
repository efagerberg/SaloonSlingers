using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    class GameRulesTests
    {
        class TestLoad
        {
            [TestCaseSource(nameof(StringEvaluatorTestCases))]
            public void UsesPassedEvaluator(string evaluatorName, IHandEvaluator expectedEvaluator)
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    HandEvaluator = evaluatorName
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(2).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = expectedEvaluator.Evaluate(hand)
                };

                Assert.AreEqual(actual.Name, config.Name);
                Assert.AreEqual(actual.Evaluate(ctx.Hand), ctx.Evaluation);
            }

            [Test]
            public void WhenInvalidHandlerString_Raises()
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    HandEvaluator = "SomeNonexistingEvaluator"
                };

                Assert.Throws<InvalidGameRulesConfig>(() => CardGame.Load(config));
            }

            [TestCaseSource(nameof(MaxHandSizeTestCases))]
            public void WhenHandsizeAtOrBelowMax_CanDraw(int maxHandSize, int handSize, bool expected)
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    HandEvaluator = "BlackJack",
                    MaxHandSize = maxHandSize
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(handSize).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = new BlackJackHandEvaluator().Evaluate(hand)
                };

                Assert.That(actual.Draw(ctx).HasValue == expected);
            }

            [TestCaseSource(nameof(MinScoreTestCases))]
            public void WhenScoreAtOrAboveMinimum_CanDraw(int minScore, int evaluationScore, bool expected)
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    MinScore = (uint)minScore,
                    HandEvaluator = "BlackJack"
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(3).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = new HandEvaluation(HandNames.NONE, (uint)evaluationScore)
                };

                Assert.That(actual.Draw(ctx).HasValue == expected);
            }

            [TestCaseSource(nameof(MaxScoreTestCases))]
            public void WhenScoreAtOrBelowMaximum_CanDraw(int maxScore, int evaluationScore, bool expected)
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    MaxScore = (uint)maxScore,
                    HandEvaluator = "BlackJack"
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(3).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = new HandEvaluation(HandNames.NONE, (uint)evaluationScore)
                };

                Assert.That(actual.Draw(ctx).HasValue == expected);
            }

            [TestCaseSource(nameof(MinMaxScoreTestCases))]
            public void WhenAllRulesAreTure_CanDraw(int minScore, int maxScore, int actualScore, bool expected)
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    MaxScore = (uint)maxScore,
                    MinScore = (uint)minScore,
                    HandEvaluator = "BlackJack"
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(3).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = new HandEvaluation(HandNames.NONE, (uint)actualScore)
                };

                Assert.That(actual.Draw(ctx).HasValue == expected);
            }

            private static readonly object[][] StringEvaluatorTestCases = {
                new object[] { "Poker", new PokerHandEvaluator() },
                new object[] { "poker", new PokerHandEvaluator() },
                new object[] { "  pokeR  ", new PokerHandEvaluator() },
                new object[] { "BLACKJACK", new BlackJackHandEvaluator() },
                new object[] { "Black  Jack", new BlackJackHandEvaluator() },
                new object[] { "wAR", new WarHandEvaluator() },
                new object[] { "War", new WarHandEvaluator() },
            };

            private static readonly object[][] MinScoreTestCases = {
                new object[] { 3, 2, false },
                new object[] { 3, 3, true },
                new object[] { 1, 5, true },
            };

            private static readonly object[][] MaxScoreTestCases = {
                new object[] { 3, 2, true },
                new object[] { 3, 3, false },
                new object[] { 1, 5, false },
            };

            private static readonly object[][] MaxHandSizeTestCases = {
                new object[] { 3, 2, true },
                new object[] { 3, 3, false },
                new object[] { 1, 5, false },
            };

            private static readonly object[][] MinMaxScoreTestCases =
            {
                new object[] { 1, 5, 3, true },
                new object[] { 2, 5, 3, true },
                new object[] { 3, 3, 3, false },
                new object[] { 1, 5, 5, false },
                new object[] { 1, 5, 7, false },
            };

            [Test]
            public void WhenRuleHasSideEffect_AndCanDraw_AppliesSideEffect()
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    HandEvaluator = "BlackJack",
                    CostTable = new uint[] { 1, 3, 5, 7 }
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(3).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = new HandEvaluation(HandNames.NONE, 100),
                    AttributeRegistry = new Dictionary<AttributeType, Points>()
                    {
                        { AttributeType.Money, new Points(10) },
                        { AttributeType.Pot, new Points(0, uint.MaxValue) }
                    }
                };

                Assert.That(actual.Draw(ctx).HasValue == true);
                Assert.That(ctx.AttributeRegistry[AttributeType.Money].Value, Is.EqualTo(3));
                Assert.That(ctx.AttributeRegistry[AttributeType.Pot].Value, Is.EqualTo(7));
            }

            [Test]
            public void WhenDrawCostsMoney_AndNotEnoughMoneyLeft_CantDraw()
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    HandEvaluator = "BlackJack",
                    CostTable = new uint[] { 1, 3, 5000 }
                };
                CardGame actual = CardGame.Load(config);
                Deck deck = new();
                var hand = deck.Draw(2).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = new HandEvaluation(HandNames.NONE, 100),
                    AttributeRegistry = new Dictionary<AttributeType, Points>()
                    {
                        { AttributeType.Money, new Points(10) },
                        { AttributeType.Pot, new Points(0, uint.MaxValue) }
                    }
                };

                Assert.That(actual.Draw(ctx).HasValue == false);
                Assert.That(ctx.AttributeRegistry[AttributeType.Money].Value, Is.EqualTo(10));
                Assert.That(ctx.AttributeRegistry[AttributeType.Pot].Value, Is.EqualTo(0));
            }
        }
    }
}
