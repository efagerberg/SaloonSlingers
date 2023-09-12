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
            [TestCaseSource(nameof(LoadTestCases))]
            public void TestReturnsExpectedGameRules(string evaluatorName, int maxHandSize, IHandEvaluator expectedEvaluator)
            {
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "MaxHandSize", maxHandSize },
                    { "HandEvaluator", evaluatorName }
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);
                Deck deck = new();
                var hand = deck.RemoveFromTop(maxHandSize).ToList();
                DrawContext ctx = new()
                {
                    Deck = deck,
                    Hand = hand,
                    Evaluation = expectedEvaluator.Evaluate(hand)
                };

                Assert.AreEqual(actual.Name, config["Name"]);
                Assert.False(actual.CanDraw(ctx));
                Assert.AreEqual(actual.Evaluate(ctx.Hand), ctx.Evaluation);
            }

            [Test]
            public void TestRaisesWhenInvalidHandlerString()
            {
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "MaxHandSize", 10 },
                    { "HandEvaluator", "SomeNonexistingEvaluator" }
                };
                string raw = JsonConvert.SerializeObject(config);

                Assert.Throws<InvalidGameRulesConfig>(() => GameRules.Load(raw));
            }

            private static readonly object[][] LoadTestCases = {
                new object[] { "Poker", 7, new PokerHandEvaluator() },
                new object[] { "poker", 4, new PokerHandEvaluator() },
                new object[] { "  pokeR  ", 5, new PokerHandEvaluator() },
                new object[] { "BLACKJACK", 2, new BlackJackHandEvaluator() },
                new object[] { "Black  Jack", 4, new BlackJackHandEvaluator() }
            };
        }
    }
}
