using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class CardTests
    {
        class TestConstructor
        {
            [Test]
            public void WithSuitAndValue_CreatesExpectedCard()
            {
                Card cardUnderTest = new(Values.JACK, Suits.HEARTS);

                Assert.AreEqual(cardUnderTest.Suit, Suits.HEARTS);
                Assert.AreEqual(cardUnderTest.Value, Values.JACK);
            }

            [Test]
            public void WithCard_CreatesExpectedCard()
            {
                Card cardParam = new(Values.JACK, Suits.HEARTS);
                Card cardUnderTest = new(cardParam);

                Assert.AreEqual(cardUnderTest.Suit, Suits.HEARTS);
                Assert.AreEqual(cardUnderTest.Value, Values.JACK);
            }

            [Test]
            public void WithString_CreatesExpectedCard()
            {
                Card cardUnderTest = new("AD");

                Assert.AreEqual(Suits.DIAMONDS, cardUnderTest.Suit);
                Assert.AreEqual(Values.ACE, cardUnderTest.Value);
            }
        }

        class TestIsFace
        {
            [TestCaseSource(nameof(IsFaceCardTestCases))]
            public void ForNonFaceCard_ReturnsTrue(Card card, bool expected)
            {
                Assert.That(card.IsFaceCard() == expected);
            }

            private static readonly object[][] IsFaceCardTestCases =
            {
                new object[] { new Card("AH"), false },
                new object[] { new Card("8D"), false },
                new object[] { new Card("TC"), false },
                new object[] { new Card("JS"), true },
                new object[] { new Card("QH"), true },
                new object[] { new Card("KD"), true },
            };
        }

        class TestToString
        {
            [Test]
            public void ToString_AceCard_ReturnsExpectedString()
            {
                Card cardUnderTest = new(Values.ACE, Suits.CLUBS);
                Assert.AreEqual(cardUnderTest.ToString(), "ace_of_clubs");
            }

            [Test]
            public void ToString_NumberCard_ReturnsExpectedString()
            {
                Card cardUnderTest = new(Values.EIGHT, Suits.CLUBS);
                Assert.AreEqual(cardUnderTest.ToString(), "8_of_clubs");
            }

            [Test]
            public void ToString_FaceCard_ReturnsExpectedString()
            {
                Card cardUnderTest = new(Values.JACK, Suits.DIAMONDS);
                Assert.AreEqual(cardUnderTest.ToString(), "jack_of_diamonds");
            }
        }

        class TestGetHashCode
        {
            [Test]
            public void ForSameCard_ReturnsSameValue()
            {
                Card card1 = new(Values.ACE, Suits.DIAMONDS);
                Card card2 = new(Values.ACE, Suits.DIAMONDS);

                Assert.AreEqual(card1.GetHashCode(), card2.GetHashCode());
            }

            [Test]
            public void ForDifferentCard_ReturnsDifferentValue()
            {
                Card card1 = new(Values.ACE, Suits.DIAMONDS);
                Card card2 = new(Values.TWO, Suits.DIAMONDS);

                Assert.AreNotEqual(card1.GetHashCode(), card2.GetHashCode());
            }
        }

        class TestEncode
        {
            public static readonly object[][] EncodeTestCases = {
                new object[] { "KD", "11010100" },
                new object[] { "QC", "11001000" },
                new object[] { "2H", "00100010" },
                new object[] { "AD", "00010100" },
                new object[] { "5S", "01010001" },
                new object[] { "TH", "10100010" }
            };

            [TestCaseSource(nameof(EncodeTestCases))]
            public void ReturnsExpectedByte_WhenEncoded(string cardString, string expected)
            {
                Card subject = new(cardString);
                byte actual = Card.Encode(subject);

                Assert.Greater(Marshal.SizeOf(subject), Marshal.SizeOf(actual));
                Assert.AreEqual(expected, TestHelpers.ConvertToBinaryString(actual, 8));
            }
        }

        class TestDecode
        {
            private static readonly IEnumerable<object[]> DecodeTestCases = TestEncode.EncodeTestCases.Select(
                x => new object[] { x[1], x[0] }
            );

            [TestCaseSource(nameof(DecodeTestCases))]
            public void ReturnsExpectedCard_WhenDecoded(string cardBinaryString, string expectedCardString)
            {
                Card expected = new(expectedCardString);
                byte binaryCard = Convert.ToByte(cardBinaryString, 2);
                Card actual = Card.Decode(binaryCard);

                Assert.Greater(Marshal.SizeOf(actual), Marshal.SizeOf(binaryCard));
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
