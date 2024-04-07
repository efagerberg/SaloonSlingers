using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HandEvaluationTests
    {
        public class TestDisplayName
        {
            [TestCaseSource(nameof(DisplayNameTestCases))]
            public void ReturnsExpectedNameString(HandEvaluation x, string expected)
            {
                Assert.AreEqual(x.DisplayName(), expected);
            }

            private static readonly object[][] DisplayNameTestCases = {
                new object[] { HandEvaluation.EMPTY, "" },
                new object[] { new HandEvaluation(HandNames.HIGH_CARD, 10), "High Card" },
                new object[] { new HandEvaluation(HandNames.BUST, 1), "Bust"},
                new object[] { new HandEvaluation(HandNames.FOUR_OF_A_KIND, 4), "Four Of A Kind"}
            };
        }
    }
}
