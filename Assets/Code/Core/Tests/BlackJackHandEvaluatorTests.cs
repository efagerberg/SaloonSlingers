using System.Collections.Generic;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class BlackJackHandEvaluatorTests : HandEvaluatorTestBase
    {
        public class EvaluateTests
        {
            private static readonly BlackJackHandEvaluator subject = new();
            private static readonly object[] EvaluateTestCases = {
                new object[] { "", 0 },
                new object[] { "8C", 8 },
                new object[] { "9H", 9 },
                new object[] { "7H JD", 17 },
                new object[] { "8C 8H", 16 },
                new object[] { "AC 8H", 19 },
                new object[] { "JC JH", 20 },
                new object[] { "JH KS TD", subject.GetMinHandValue() },
                new object[] { "JC AH", subject.GetMaxHandValue() },
                new object[] { "AD AH AS", 13 },
                new object[] { "AD 8H 5H", 14 }
            };

            [TestCaseSource(nameof(EvaluateTestCases))]
            public void ReturnsExpectedResult(string handString, float expectedHandValue)
            {
                Assert.AreEqual(expectedHandValue, subject.Evaluate(MakeHandFromString(handString)));
            }
        }
    }
};