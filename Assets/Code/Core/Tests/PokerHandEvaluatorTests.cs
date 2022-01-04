using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class PokerHandEvaluatorTests
    {
        public class EvaluateTests
        {
            private static readonly PokerHandEvaluator subject = new();
            private static readonly object[][] EvaluateTestCases = {
                new object[] { "", "", "AreEqual" },
                new object[] { "2H", "", "Greater" },
                new object[] { "2D", "2D 3S", "Less" },
                new object[] { "2D 3S 5D", "2D 5D TS", "Less" },
                new object[] { "AH", "2D 5D TS", "Greater" },
                new object[] { "AH 3H", "3H KD", "Greater" },
                new object[] { "3H AH", "3H KD", "Greater" },
                new object[] { "AH KD", "AD KS QS", "Less" },
                new object[] { "AH 9C 3S 5C 2D", "AH 9C 3S 5C", "Greater" }
            };

            private static int EvaluateHandString(string x)
            {
                return subject.Evaluate(TestHelpers.MakeHandFromString(x));
            }

            [TestCaseSource(nameof(EvaluateTestCases))]
            public void ReturnsExpectedResult(string firstHand, string secondHand, string assertionMethod)
            {
                TestHelpers.GetAssertionFromMethodString(assertionMethod)(
                    EvaluateHandString(firstHand),
                    EvaluateHandString(secondHand)
                );
            }
        }
    }
};