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
                new object[]
                {
                    new Card[] { new Card(Suits.DIAMONDS, Values.EIGHT) },
                    1.08f
                },
                new object[]
                {
                    new Card[] { new Card(Suits.DIAMONDS, Values.KING)},
                    1.13f
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.DIAMONDS, Values.KING),
                        new Card(Suits.DIAMONDS, Values.EIGHT)
                    },
                    1.13f
                },
                new object[]
                {
                    new Card[] { new Card(Suits.DIAMONDS, Values.ACE) },
                    1.14f
                },
                new object[]
                {
                    new Card[]
                    {
                        new Card(Suits.HEARTS, Values.FIVE),
                        new Card(Suits.CLUBS, Values.JACK),
                        new Card(Suits.SPADES, Values.ACE)
                    },
                    1.14f
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.FOUR),
                        new Card(Suits.CLUBS, Values.FOUR)
                    },
                    2.04f
                },
                new object[]
                {
                    new Card[]
                    {
                        new Card(Suits.DIAMONDS, Values.KING),
                        new Card(Suits.DIAMONDS, Values.KING),
                        new Card(Suits.DIAMONDS, Values.JACK),
                        new Card(Suits.DIAMONDS, Values.JACK),
                    },
                    3.24f
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.TEN),
                        new Card(Suits.CLUBS, Values.TEN),
                        new Card(Suits.CLUBS, Values.TEN)
                    },
                    4.1f
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.ACE),
                        new Card(Suits.HEARTS, Values.ACE),
                        new Card(Suits.SPADES, Values.ACE)
                    },
                    4.14f
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.ACE),
                        new Card(Suits.HEARTS, Values.ACE),
                        new Card(Suits.SPADES, Values.ACE),
                        new Card(Suits.DIAMONDS, Values.KING),
                        new Card(Suits.HEARTS, Values.KING)
                    },
                    7.14f
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.ACE),
                        new Card(Suits.HEARTS, Values.ACE),
                        new Card(Suits.SPADES, Values.ACE),
                        new Card(Suits.DIAMONDS, Values.ACE)
                    },
                    8.14f
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