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
            public void CalculatesHandScore(string firstHand, string secondHand, string assertionMethod)
            {
                HandEvaluation firstEvaluation = TestHelpers.EvaluateHandString(firstHand, subject);
                HandEvaluation secondEvaluation = TestHelpers.EvaluateHandString(secondHand, subject);
                TestHelpers.GetAssertionFromMethodString<uint>(assertionMethod)(
                    firstEvaluation.Score,
                    secondEvaluation.Score
                );
            }

            [TestCase("AH", "")]
            [TestCase("AH 2C", "")]
            [TestCase("JH QC", "")]
            public void ReturnsNoKeyCards(string hand, string expectedKeyCards)
            {
                HandEvaluation evaluation = TestHelpers.EvaluateHandString(hand, subject);
                CollectionAssert.AreEqual(evaluation.KeyIndexes, TestHelpers.MakeHandFromString(expectedKeyCards));
            }

            [TestCase("", HandNames.NONE)]
            [TestCase("AH", HandNames.ELEVEN)]
            [TestCase("AH AH", HandNames.TWELVE)]
            [TestCase("JS JC", HandNames.TWENTY)]
            [TestCase("AH JC JC", HandNames.TWENTY_ONE)]
            [TestCase("AH JS", HandNames.BLACK_JACK)]
            [TestCase("JS JC QH", HandNames.BUST)]
            public void ReturnsExpectedHandName(string hand, HandNames expected)
            {
                Assert.AreEqual(expected, TestHelpers.EvaluateHandString(hand, subject).Name);
            }

            private static readonly BlackJackHandEvaluator subject = new();
        }
    }
};