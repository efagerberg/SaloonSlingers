using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class BlackJackHandEvaluatorTests
    {
        public class EvaluateTests
        {
            private static readonly BlackJackHandEvaluator subject = new();
            private static readonly object[][] EvaluateTestCases = {
                new object[] { "", "", "AreEqual" },
                new object[] { "2H", "", "Greater" },
                new object[] { "2H", "2D", "AreEqual" },
                new object[] { "AD", "KS", "Greater"},
                new object[] { "JC QD TH", "", "Less" },
                new object[] { "AH AD JS", "AD KS", "Less" },
                new object[] { "AH AC AS", "AH", "Greater" }
            };

            private static uint EvaluateHandString(string x)
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