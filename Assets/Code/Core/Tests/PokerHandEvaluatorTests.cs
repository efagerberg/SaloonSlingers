using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class PokerHandEvaluatorTests
    {
        public class EvaluateTests
        {
            private static readonly PokerHandEvaluator subject = new();
            private static readonly object[] EvaluateTestCases = {
                new object[] {
                    new Card[] { },
                    0
                },
            };

            [TestCaseSource(nameof(EvaluateTestCases))]
            public void ReturnsExpectedResult(Card[] hand, float expectedResult)
            {
                Assert.AreEqual(expectedResult, subject.Evaluate(hand));
            }
        }
    }
};