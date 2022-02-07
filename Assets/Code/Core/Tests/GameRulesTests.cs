using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                using MemoryStream stream = MakeConfigStream(config);
                var actual = GameRules.Load(stream);

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
                using MemoryStream stream = MakeConfigStream(config);

                Assert.Throws<InvalidGameRulesConfig>(() => GameRules.Load(stream));
            }

            private static MemoryStream MakeConfigStream(Dictionary<string, object> config)
            {
                MemoryStream stream = new();
                using StreamWriter writer = new(stream, Encoding.UTF8, WRITE_BUFFER_SIZE, true);
                writer.Write(JsonConvert.SerializeObject(config));
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            private const int WRITE_BUFFER_SIZE = 2048;
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
