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
                var cardUnderTest = new Card(Values.JACK, Suits.HEARTS);

                Assert.AreEqual(cardUnderTest.Suit, Suits.HEARTS);
                Assert.AreEqual(cardUnderTest.Value, Values.JACK);
            }

            [Test]
            public void WithCard_CreatesExpectedCard()
            {
                var cardParam = new Card(Values.JACK, Suits.HEARTS);
                var cardUnderTest = new Card(cardParam);

                Assert.AreEqual(cardUnderTest.Suit, Suits.HEARTS);
                Assert.AreEqual(cardUnderTest.Value, Values.JACK);
            }

            [Test]
            public void WithString_CreatesExpectedCard()
            {
                var cardUnderTest = new Card("AD");

                Assert.AreEqual(Suits.DIAMONDS, cardUnderTest.Suit);
                Assert.AreEqual(Values.ACE, cardUnderTest.Value);
            }
        }

        class TestIsFace
        {
            [Test]
            public void ForNonFaceCard_ReturnsTrue()
            {
                var cardUnderTest = new Card(Values.EIGHT, Suits.DIAMONDS);
                Assert.IsFalse(cardUnderTest.IsFaceCard());

                cardUnderTest.Value = Values.ACE;
                Assert.IsFalse(cardUnderTest.IsFaceCard());
            }

            [Test]
            public void ForFaceCards_ReturnsTrue()
            {
                var cardUnderTest = new Card(Values.QUEEN, Suits.CLUBS);
                Assert.IsTrue(cardUnderTest.IsFaceCard());

                cardUnderTest.Value = Values.KING;
                Assert.IsTrue(cardUnderTest.IsFaceCard());

                cardUnderTest.Value = Values.JACK;
                Assert.IsTrue(cardUnderTest.IsFaceCard());
            }
        }

        class TestToString
        {
            [Test]
            public void ToString_AceCard_ReturnsExpectedString()
            {
                var cardUnderTest = new Card(Values.ACE, Suits.CLUBS);
                Assert.AreEqual(cardUnderTest.ToString(), "ace_of_clubs");
            }

            [Test]
            public void ToString_NumberCard_ReturnsExpectedString()
            {
                var cardUnderTest = new Card(Values.EIGHT, Suits.CLUBS);
                Assert.AreEqual(cardUnderTest.ToString(), "8_of_clubs");
            }

            [Test]
            public void ToString_FaceCard_ReturnsExpectedString()
            {
                var cardUnderTest = new Card(Values.JACK, Suits.DIAMONDS);
                Assert.AreEqual(cardUnderTest.ToString(), "jack_of_diamonds");
            }
        }

        class TestGetHashCode
        {
            [Test]
            public void ForSameCard_ReturnsSameValue()
            {
                var card1 = new Card(Values.ACE, Suits.DIAMONDS);
                var card2 = new Card(Values.ACE, Suits.DIAMONDS);

                Assert.AreEqual(card1.GetHashCode(), card2.GetHashCode());
            }

            [Test]
            public void ForDifferentCard_ReturnsDifferentValue()
            {
                var card1 = new Card(Values.ACE, Suits.DIAMONDS);
                var card2 = new Card(Values.TWO, Suits.DIAMONDS);

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
                Card subject = new Card(cardString);
                byte actual = Card.Encode(subject);

                Assert.Greater(Marshal.SizeOf(subject), Marshal.SizeOf(actual));
                Assert.AreEqual(expected, ConvertToBinaryString(actual));
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
                Card expected = new Card(expectedCardString);
                byte binaryCard = Convert.ToByte(cardBinaryString, 2);
                Card actual = Card.Decode(binaryCard);

                Assert.Greater(Marshal.SizeOf(actual), Marshal.SizeOf(binaryCard));
                Assert.AreEqual(expected, actual);
            }
        }

        private static string ConvertToBinaryString(byte x)
        {
            return Convert.ToString(x, 2).PadLeft(8, '0');
        }
    }
}
