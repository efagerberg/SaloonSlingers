using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class WarHandEvaluatorTests
    {
        public class EvaluateTests
        {
            [TestCase("", "", "AreEqual")]
            [TestCase("2H", "", "Greater")]
            [TestCase("2H", "2D", "AreEqual")]
            [TestCase("AD", "KS", "Less")]
            [TestCase("JC QD TH", "", "Greater")]
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
            [TestCase("AH", HandNames.NONE)]
            [TestCase("AH AH", HandNames.NONE)]
            [TestCase("JS JC", HandNames.NONE)]
            [TestCase("AH JC JC", HandNames.NONE)]
            [TestCase("AH JS", HandNames.NONE)]
            [TestCase("JS JC QH", HandNames.NONE)]
            public void ReturnsExpectedHandName(string hand, HandNames expected)
            {
                Assert.AreEqual(expected, TestHelpers.EvaluateHandString(hand, subject).Name);
            }

            private static readonly WarHandEvaluator subject = new();
        }
    }
};