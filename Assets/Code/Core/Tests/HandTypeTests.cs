using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HandTypeTests
    {
        public class TestDisplayName
        {
            [TestCaseSource(nameof(DisplayNameTestCases))]
            public void TestReturnsExpectedNameString(HandType x, string expected)
            {
                Assert.AreEqual(x.DisplayName(), expected);
            }

            private static object[][] DisplayNameTestCases =
            {
                new object[] { new HandType(HandNames.EMPTY, 0), "" },
                new object[] { new HandType(HandNames.HIGH_CARD, 10), "High Card" },
                new object[] { new HandType(HandNames.BUST, 1), "Bust"},
                new object[] { new HandType(HandNames.FOUR_OF_A_KIND, 4), "Four Of A Kind"}
            };
        }
    }
}
