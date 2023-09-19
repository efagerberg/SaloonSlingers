using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

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
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "HandEvaluator", evaluatorName }
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);
                Deck deck = new();
                var hand = deck.RemoveFromTop(2).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = expectedEvaluator.Evaluate(hand)
                };

                Assert.AreEqual(actual.Name, config["Name"]);
                Assert.AreEqual(actual.Evaluate(ctx.Hand), ctx.Evaluation);
            }

            [Test]
            public void TestRaisesWhenInvalidHandlerString()
            {
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "HandEvaluator", "SomeNonexistingEvaluator" }
                };
                string raw = JsonConvert.SerializeObject(config);

                Assert.Throws<InvalidGameRulesConfig>(() => GameRules.Load(raw));
            }

            [TestCaseSource(nameof(MaxHandSizeTestCases))]
            public void TestCanOnlyDrawWhenHandsizeAtOrBelowMax(int maxHandSize, int handSize, bool expected)
            {
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "HandEvaluator", "BlackJack" },
                    { "MaxHandSize", maxHandSize },
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);
                Deck deck = new();
                var hand = deck.RemoveFromTop(handSize).ToList();
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
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "MinScore", minScore },
                    { "HandEvaluator", "BlackJack" }
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);
                Deck deck = new();
                var hand = deck.RemoveFromTop(3).ToList();
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
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "MaxScore", maxScore },
                    { "HandEvaluator", "BlackJack" }
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);
                Deck deck = new();
                var hand = deck.RemoveFromTop(3).ToList();
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
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "MaxScore", maxScore },
                    { "MinScore", minScore },
                    { "HandEvaluator", "BlackJack" }
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);
                Deck deck = new();
                var hand = deck.RemoveFromTop(3).ToList();
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
                new object[] { "Black  Jack", new BlackJackHandEvaluator() }
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
