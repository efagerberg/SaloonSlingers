﻿using System.Linq;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    class GameRulesTests
    {
        class TestLoad
        {
            [TestCaseSource(nameof(StringEvaluatorTestCases))]
            public void TestEvaluateUsesPassedEvaluator(string evaluatorName,
                                                        IHandEvaluator expectedEvaluator)
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
            public void TestRaisesWhenInvalidHandlerString()
            {
                CardGameConfig config = new()
                {
                    Name = "TestGame",
                    HandEvaluator = "SomeNonexistingEvaluator"
                };

                Assert.Throws<InvalidGameRulesConfig>(() => CardGame.Load(config));
            }

            [TestCaseSource(nameof(MaxHandSizeTestCases))]
            public void TestCanOnlyDrawWhenHandsizeAtOrBelowMax(int maxHandSize, int handSize, bool expected)
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

                Assert.That(actual.CanDraw(ctx) == expected);
            }

            [TestCaseSource(nameof(MinScoreTestCases))]
            public void TestCanOnlyDrawWhenScoreAtOrAboveMinimum(int minScore, int evaluationScore, bool expected)
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

                Assert.That(actual.CanDraw(ctx) == expected);
            }

            [TestCaseSource(nameof(MaxScoreTestCases))]
            public void TestCanOnlyDrawWhenScoreAtOrBelowMaximum(int maxScore, int evaluationScore, bool expected)
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

                Assert.That(actual.CanDraw(ctx) == expected);
            }

            [TestCaseSource(nameof(MinMaxScoreTestCases))]
            public void TestCanOnlyDrawWhenAllRulesAreTure(int minScore, int maxScore, int actualScore, bool expected)
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

                Assert.That(actual.CanDraw(ctx) == expected);
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
        }
    }
}
