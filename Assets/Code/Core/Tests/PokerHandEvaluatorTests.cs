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
                new object[] { "AH 9C 3S 5C 2D", "AH 9C 3S 5C", "Greater" },
                new object[] { "2C 2S", "2C KH", "Greater" },
                new object[] { "2C 2S", "AC AH", "Less" },
                new object[] { "2C 2H 3S 3D", "KH KD", "Greater" },
                new object[] { "AH AD", "AS AC", "AreEqual" },
                new object[] { "KH KD AC", "KS KC AD 2H", "Less" },
                new object[] { "2C 2H 2D", "AH AD 2C", "Greater" },
                new object[] { "2C 2D 2S AD", "3C 3H 3D", "Less" },
                new object[] { "2C 2D 2S", "3C 3S 2C AH KD", "Greater" },
                new object[] { "2H 3D 4S 5D 6C", "AH AD AC", "Greater" },
                new object[] { "2H 3D 4S 5D 6C", "3D 4C 5S 6D 7C", "Less" },
                new object[] { "AH 2C 3D 4S 5H", "KH KD KS", "Greater"},
                new object[] { "AH 2C 3D 4S 5H", "AH 2C 3D 4S 5H 8D TS", "Less" },
                new object[] { "8D 9C TS JH KS", "2H 4H 6H 3H 8H", "Less" },
                new object[] { "2H 4H 6H 3H 8H", "TD 6D 5D 3D 8D", "Less" },
                new object[] { "2H 3H 5H TH JH AD", "2D 3D 5D TD JD AC 2C", "Less" },
                new object[] { "2H 3H 5H TH JH AD", "2D 3D 5D TD KD 2C", "Less" },
                new object[] { "2C 2H 3D 3C 3S", "2H 3H 5H TH JH AD", "Greater" },
                new object[] { "2C 2H 3D 3C 3S", "AH AD AS 2H 2D", "Less" },
                new object[] { "2C 2H 3D 3C 3S 4D", "2C 2H 3D 3C 3S AD", "Less" },
                new object[] { "AH AS 3H 3D 3S", "2C 2H 4H 4D 4S", "Less" },
                new object[] { "2H 2D 2S 2C 3D", "AH AD AS 2H 2D", "Greater" },
                new object[] { "4D 4C 4S 4H 2S", "AH AD AS AC 2S", "Less" },
                new object[] { "AH AD AS AC 2S", "2D 3D 4D 5D 6D", "Less" },
                new object[] { "3D 4D 5D 6D 7D AH", "3S 4S 5S 6S 7S KH", "Greater" },
                new object[] { "TD JD QD KD AD", "3D 4D 5D 6D 7D AH", "Greater" },
                new object[] { "TD JD QD KD AD", "TS JS QS KS AS", "AreEqual" },
                new object[] { "TD JD QD KD AD", "TS JS QS KS AS AC", "Less" },
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