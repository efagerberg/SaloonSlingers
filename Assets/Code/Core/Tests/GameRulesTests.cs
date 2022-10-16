using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using NUnit.Framework;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core.Tests
{
    class GameRulesTests
    {
        class TestLoad
        {
            [TestCaseSource(nameof(LoadTestCases))]
            public void TestReturnsExpectedGameRules(string evaluatorName, int maxHandSize, Type expectedEvaluatorType)
            {
                Dictionary<string, object> config = new()
                {
                    { "Name", "TestConfig" },
                    { "MaxHandSize", maxHandSize },
                    { "HandEvaluator", evaluatorName }
                };
                string raw = JsonConvert.SerializeObject(config);
                GameRules actual = GameRules.Load(raw);

                Assert.AreEqual(actual.Name, config["Name"]);
                Assert.AreEqual(actual.MaxHandSize, config["MaxHandSize"]);
                Assert.IsInstanceOf(expectedEvaluatorType, actual.HandEvaluator);
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
                new object[] { "Poker", 7, typeof(PokerHandEvaluator) },
                new object[] { "poker", 4, typeof(PokerHandEvaluator) },
                new object[] { "  pokeR  ", 5, typeof(PokerHandEvaluator) },
                new object[] { "BLACKJACK", 2, typeof(BlackJackHandEvaluator) },
                new object[] { "Black  Jack", 4, typeof(BlackJackHandEvaluator) }
            };
        }
    }
}
