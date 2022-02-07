using NUnit.Framework;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core.Tests
{
    public class BlackJackHandEvaluatorTests
    {
        public class EvaluateTests
        {
            [TestCase("", "", "AreEqual")]
            [TestCase("2H", "", "Greater")]
            [TestCase("2H", "2D", "AreEqual")]
            [TestCase("AD", "KS", "Greater")]
            [TestCase("JC QD TH", "", "Less")]
            [TestCase("AH AD JS", "AD KS", "Less")]
            [TestCase("AH AC AS", "AH", "Greater")]
            public void ReturnsExpectedResult(string firstHand, string secondHand, string assertionMethod)
            {
                TestHelpers.GetAssertionFromMethodString(assertionMethod)(
                    EvaluateHandString(firstHand),
                    EvaluateHandString(secondHand)
                );
            }

            private static readonly BlackJackHandEvaluator subject = new();

            private static uint EvaluateHandString(string x)
            {
                return subject.Evaluate(TestHelpers.MakeHandFromString(x)).Score;
            }
        }
    }
};