using System;
using System.Linq;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class PokerHandEvaluatorTests
    {
        public class EvaluateTests
        {
            private static readonly PokerHandEvaluator subject = new();

            [TestCase("AH 2H 3H 4H 5H 6H 7H 8H")]
            [TestCase("AH 2H 3H 4H 5H 6H 7H 8H 9H")]
            [TestCase("AH 2H 3H 4H 5H 6H 7H 8H 9H TH")]
            public void ThrowsWhenInvalidHandSize(string handString)
            {
                Assert.Throws<ArgumentException>(() => TestHelpers.EvaluateHandString(handString, subject));
            }

            [TestCaseSource(nameof(HighCardTestCases))]
            [TestCaseSource(nameof(PairTestCases))]
            [TestCaseSource(nameof(TwoPairTestCases))]
            [TestCaseSource(nameof(ThreeOfAKindTestCases))]
            [TestCaseSource(nameof(StraightTestCases))]
            [TestCaseSource(nameof(FlushTestCases))]
            [TestCaseSource(nameof(FullHouseTestCase))]
            [TestCaseSource(nameof(FourOfAKindTestCases))]
            [TestCaseSource(nameof(StraightFlushCases))]
            [TestCaseSource(nameof(RoyalFlushTestCases))]
            public void ReturnsExpectedScore(string firstHand, string secondHand, string assertionMethod)
            {
                HandEvaluation x = TestHelpers.EvaluateHandString(firstHand, subject);
                HandEvaluation y = TestHelpers.EvaluateHandString(secondHand, subject);
                TestHelpers.GetAssertionFromMethodString<uint>(assertionMethod)(x.Score, y.Score);
            }

            private static readonly object[][] HighCardTestCases = {
                new object[] { "", "", "AreEqual" },
                new object[] { "2H", "", "Greater" },
                new object[] { "2D", "2D 3S", "Less" },
                new object[] { "2D 3S 5D", "2D 5D TS", "Less" },
                new object[] { "AH", "2D 5D TS", "Greater" },
                new object[] { "AH 3H", "3H KD", "Greater" },
                new object[] { "3H AH", "3H KD", "Greater" },
                new object[] { "AH KD", "AD KS QS", "Less" },
                new object[] { "AH 9C 3S 5C 2D", "AH 9C 3S 5C", "Greater" },
                new object[] { "AH KD 9C 3S 5C 6D", "AH KD 9C 3S 5C 6D 2D", "AreEqual" },
            };
            private static readonly object[][] PairTestCases = {
                new object[] { "2C 2S", "2C KH", "Greater" },
                new object[] { "2C 2S", "AC AH", "Less" },
                new object[] { "AH AD", "AS AC", "AreEqual" },
                new object[] { "KH KD AC", "KS KC AD 2H", "Less" },
                new object[] { "AH AD", "KD KH", "Greater" },
                new object[] { "TD TH 2D 4C KD", "TC TS 2S 5H KS", "Less" },
                new object[] { "TD TH 2D 4C KD QH", "TD TH 2D 4C KD QH 3S", "AreEqual" }
            };
            private static readonly object[][] TwoPairTestCases = {
                new object[] { "2C 2H 3S 3D", "KH KD", "Greater" },
                new object[] { "KS KD 2S 2C", "TH TD KH KC", "Less" },
                new object[] { "AH AC 2H 2C", "KH KC QH QC", "Greater" },
                new object[] { "AH AC 2H 5C 3D", "AD AS 5S 2D", "Greater" },
                new object[] { "2H 2C 3C 3S KD", "2D 2S 3D 3H AH", "Less" },
                new object[] { "2H 2C 3C 3S KD JD", "2H 2C 3C 3S KD QS", "AreEqual" }
            };
            private static readonly object[][] ThreeOfAKindTestCases = {
                new object[] { "2C 2H 2D", "AH AD 2C", "Greater" },
                new object[] { "2C 2D 2S AD", "3C 3H 3D", "Less" },
                new object[] { "2C 2D 2S", "3C 3S 2C AH KD", "Greater" },
                new object[] { "2C 2D 2S 3H 4H", "2C 2D 2S 3H 5H", "Less" },
                new object[] { "2C 2D 2S 5H 4H", "2C 2D 2S 5H 4H 3S", "AreEqual" }
            };
            private static readonly object[][] StraightTestCases = {
                new object[] { "2H 3D 4S 5D 6C", "AH AD AC", "Greater" },
                new object[] { "2H 3D 4S 5D 6C", "3D 4C 5S 6D 7C", "Less" },
                new object[] { "AH 2C 3D 4S 5H", "KH KD KS", "Greater"},
                new object[] { "AH 2C 3D 4S 5H", "AH 2C 3D 4S 5H 8D TS", "AreEqual" },
                new object[] { "AH 2C 3D 4S 5H", "AH KS QC JD TH", "Less" }
            };
            private static readonly object[][] FlushTestCases = {
                new object[] { "8D 9C TS JH KS", "2H 4H 6H 3H 8H", "Less" },
                new object[] { "2H 4H 6H 3H 8H", "TD 6D 5D 3D 8D", "Less" },
                new object[] { "2H 3H 5H TH JH AD", "2D 3D 5D TD JD AC 2C", "AreEqual" },
                new object[] { "2H 3H 5H TH JH", "2D 3D 5D TD KD", "Less" },
                new object[] { "AH 3H 5H TH JH", "2H 3H 5H TH JH", "Greater" },
                new object[] { "AH 3H 5H TH JH", "AH 3H 5H TH JH TD", "AreEqual" },
            };
            private static readonly object[][] FullHouseTestCase = {
                new object[] { "2C 2H 3D 3C 3S", "2H 3H 5H TH JH", "Greater" },
                new object[] { "2C 2H 3D 3C 3S", "AH AD AS 2H 2D", "Less" },
                new object[] { "2C 2H 3D 3C 3S 4D", "2C 2H 3D 3C 3S AD", "AreEqual" },
                new object[] { "AH AS 3H 3D 3S", "2C 2H 4H 4D 4S", "Less" },
                new object[] { "AH AS 3H 3D 3S", "AH AS 3H 3D 3S JD", "AreEqual" },
                new object[] { "AH AS AD 3H 3D 3S", "AH AS 3H 3D 3S JD", "Greater" },
                new object[] { "3H 3D 3S AH AS AD", "4H 4D 4S AH AS AD", "Less" }
            };
            private static readonly object[][] FourOfAKindTestCases = {
                new object[] { "2H 2D 2S 2C 3D", "AH AD AS 2H 2D", "Greater" },
                new object[] { "4D 4C 4S 4H 2S", "AH AD AS AC 2S", "Less" },
                new object[] { "AH AD AS AC 2S", "2D 3D 4D 5D 6D", "Less" },
                new object[] { "2H 2D 2S 2C AH", "2H 2D 2S 2C KH", "Greater" },
                new object[] { "2H 2D 2S 2C AH", "2H 2D 2S 2C AH KH", "AreEqual" }
            };
            private static readonly object[][] StraightFlushCases = {
                new object[] { "AH AD AS AC 2S", "3D 4D 5D 6D 7D", "Less" },
                new object[] { "3D 4D 5D 6D 7D AH", "3S 4S 5S 6S 7S KH", "AreEqual" },
                new object[] { "4D 5D 6D 7D 8D", "5S 6S 7S 8S 9S", "Less" },
                new object[] { "4D 5D 6D 7D 8D", "4D 5D 6D 7D 8D TS", "AreEqual" }
            };
            private static readonly object[][] RoyalFlushTestCases = {
                new object[] { "TD JD QD KD AD", "3D 4D 5D 6D 7D AH", "Greater" },
                new object[] { "TD JD QD KD AD", "TS JS QS KS AS", "AreEqual" },
                new object[] { "TD JD QD KD AD", "TS JS QS KS AS AC", "AreEqual" },
            };

            [TestCase("AH", "AH")]
            [TestCase("AH JS", "AH")]
            [TestCase("JS JC QH", "JS JC")]
            [TestCase("JS JC AH AD", "JS JC AH AD")]
            [TestCase("JS JC JH", "JS JC JH")]
            [TestCase("AH 2C 3H 4D 5H", "AH 2C 3H 4D 5H")]
            [TestCase("AH 2C 3H 4D 5H KH", "AH 2C 3H 4D 5H")]
            [TestCase("AH 3H 5H TH 8H", "AH 3H 5H TH 8H")]
            [TestCase("JC JS AH AD AS", "JC JS AH AD AS")]
            [TestCase("AH 2H 3H 4H 5H", "AH 2H 3H 4H 5H")]
            [TestCase("TH JH KH QH AH", "TH JH KH QH AH")]
            [TestCase("AH AC AS QH QC KH", "AH AC AS QH QC")]
            [TestCase("AH AC AS QH QC QD", "AH AC AS QH QC")]
            public void ReturnsExpectedKeyCards(string handString, string expectedKeyCards)
            {
                var hand = TestHelpers.MakeHandFromString(handString).ToList();
                HandEvaluation evaluation = subject.Evaluate(hand);
                CollectionAssert.AreEquivalent(TestHelpers.MakeHandFromString(expectedKeyCards),
                                               evaluation.KeyIndexes.Select(i => hand[i]));
            }

            [TestCase("AH", HandNames.HIGH_CARD)]
            [TestCase("AH JS", HandNames.HIGH_CARD)]
            [TestCase("JS JC QH", HandNames.PAIR)]
            [TestCase("JS JC AH AD", HandNames.TWO_PAIR)]
            [TestCase("JS JC JH", HandNames.THREE_OF_A_KIND)]
            [TestCase("AH 2C 3H 4D 5H", HandNames.STRAIGHT)]
            [TestCase("AH 3H 5H TH 8H", HandNames.FLUSH)]
            [TestCase("JC JS AH AD AS", HandNames.FULL_HOUSE)]
            [TestCase("AH AC AS QH QC KD", HandNames.FULL_HOUSE)]
            [TestCase("AH AC AS QH QC QD", HandNames.FULL_HOUSE)]
            [TestCase("AH 2H 3H 4H 5H", HandNames.STRAIGHT_FLUSH)]
            [TestCase("9H TH JH QH KH", HandNames.STRAIGHT_FLUSH)]
            [TestCase("TH JH KH QH AH", HandNames.ROYAL_FLUSH)]
            [TestCase("9H TH JH QH KH AH", HandNames.ROYAL_FLUSH)]
            public void ReturnsExpectedHandName(string hand, HandNames expected)
            {
                Assert.AreEqual(expected, TestHelpers.EvaluateHandString(hand, subject).Name);
            }
        }
    }
};
