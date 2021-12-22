using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class BlackJackHandEvaluatorTests
    {
        public class EvaluateTests
        {
            private static readonly BlackJackHandEvaluator subject = new();
            private static readonly object[] EvaluateTestCases = {
                new object[] {
                    new Card[] {},
                    0
                },
                new object[]
                {
                    new Card[] { new Card(Suits.CLUBS, Values.EIGHT)},
                    8
                },
                new object[]
                {
                    new Card[] { new Card(Suits.HEARTS, Values.NINE) },
                    9
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.HEARTS, Values.SEVEN),
                        new Card(Suits.DIAMONDS, Values.JACK)
                    },
                    17
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.EIGHT),
                        new Card(Suits.HEARTS, Values.EIGHT)
                    },
                    16
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.ACE),
                        new Card(Suits.HEARTS, Values.EIGHT)
                    },
                    19
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.JACK),
                        new Card(Suits.HEARTS, Values.JACK)
                    },
                    20
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.HEARTS, Values.JACK),
                        new Card(Suits.SPADES, Values.KING),
                        new Card(Suits.DIAMONDS, Values.TEN)
                    },
                    subject.GetMinHandValue()
                },
                new object[]
                {
                    new Card[] {
                        new Card(Suits.CLUBS, Values.JACK),
                        new Card(Suits.HEARTS, Values.ACE)
                    },
                    subject.GetMaxHandValue()
                },
                new object[]
                {
                    new Card[]
                    {
                        new Card(Suits.DIAMONDS, Values.ACE),
                        new Card(Suits.HEARTS, Values.ACE),
                        new Card(Suits.SPADES, Values.ACE)
                    },
                    13
                },
                new object[]
                {
                    new Card[]
                    {
                        new Card(Suits.DIAMONDS, Values.ACE),
                        new Card(Suits.HEARTS, Values.EIGHT),
                        new Card(Suits.HEARTS, Values.FIVE)
                    },
                    14
                }
            };

            [TestCaseSource(nameof(EvaluateTestCases))]
            public void ReturnsExpectedResult(Card[] hand, float expectedHandValue)
            {
                Assert.AreEqual(expectedHandValue, subject.Evaluate(hand));
            }
        }
    }
};