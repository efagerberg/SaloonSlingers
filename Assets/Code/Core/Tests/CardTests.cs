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
                var cardUnderTest = new Card(Suits.HEARTS, Values.JACK);

                Assert.AreEqual(cardUnderTest.Suit, Suits.HEARTS);
                Assert.AreEqual(cardUnderTest.Value, Values.JACK);
            }

            [Test]
            public void WithCard_CreatesExpectedCard()
            {
                var cardParam = new Card(Suits.HEARTS, Values.JACK);
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


        public class TestIsFace
        {
            [Test]
            public void ForNonFaceCard_ReturnsTrue()
            {
                var cardUnderTest = new Card(Suits.DIAMONDS, Values.EIGHT);
                Assert.IsFalse(cardUnderTest.IsFaceCard());

                cardUnderTest.Value = Values.ACE;
                Assert.IsFalse(cardUnderTest.IsFaceCard());
            }

            [Test]
            public void ForFaceCards_ReturnsTrue()
            {
                var cardUnderTest = new Card(Suits.CLUBS, Values.QUEEN);
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
                var cardUnderTest = new Card(Suits.CLUBS, Values.ACE);
                Assert.AreEqual(cardUnderTest.ToString(), "ace_of_clubs");
            }

            [Test]
            public void ToString_NumberCard_ReturnsExpectedString()
            {
                var cardUnderTest = new Card(Suits.CLUBS, Values.EIGHT);
                Assert.AreEqual(cardUnderTest.ToString(), "8_of_clubs");
            }

            [Test]
            public void ToString_FaceCard_ReturnsExpectedString()
            {
                var cardUnderTest = new Card(Suits.DIAMONDS, Values.JACK);
                Assert.AreEqual(cardUnderTest.ToString(), "jack_of_diamonds");
            }
        }

        public class TestGetHashCode
        {
            [Test]
            public void ForSameCard_ReturnsSameValue()
            {
                var card1 = new Card(Suits.DIAMONDS, Values.ACE);
                var card2 = new Card(Suits.DIAMONDS, Values.ACE);

                Assert.AreEqual(card1.GetHashCode(), card2.GetHashCode());
            }

            [Test]
            public void ForDifferentCard_ReturnsDifferentValue()
            {
                var card1 = new Card(Suits.DIAMONDS, Values.ACE);
                var card2 = new Card(Suits.DIAMONDS, Values.TWO);

                Assert.AreNotEqual(card1.GetHashCode(), card2.GetHashCode());
            }
        }
    }

}
