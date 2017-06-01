using NUnit.Framework;

public class CardTests
{
    [Test]
    public void Constructor_Default_CreatesExpectedCard()
    {
        var cardUnderTest = new Card();

        Assert.AreEqual(cardUnderTest.Suit, Suits.CLUBS);
        Assert.AreEqual(cardUnderTest.Value, Values.ACE);
    }

    [Test]
    public void Constructor_Params_CreatesExpectedCard()
    {
        var cardUnderTest = new Card(Suits.HEARTS, Values.JACK);

        Assert.AreEqual(cardUnderTest.Suit, Suits.HEARTS);
        Assert.AreEqual(cardUnderTest.Value, Values.JACK);
    }

    [Test]
    public void Equals_ForSameValuedCard_ReturnsTrue()
    {
        Card cardUnderTest, cardToCompare;
        cardUnderTest = cardToCompare = new Card();
        Assert.IsTrue(cardUnderTest.Equals(cardToCompare));
    }

    [Test]
    public void Equals_ForDifferentCards_ReturnsFalse()
    {
        Card cardUnderTest, cardToCompare;
        cardUnderTest = new Card();
        cardToCompare = new Card(Suits.HEARTS, Values.JACK);
        Assert.IsFalse(cardUnderTest.Equals(cardToCompare));
    }

    [Test]
    public void Equals_ForNull_ReturnsFalse()
    {
        var cardUnderTest = new Card();
        Assert.IsFalse(cardUnderTest.Equals(null));
    }

    [Test]
    public void IsFaceCard_ForNonFaceCard_ReturnsTrue()
    {
        var cardUnderTest = new Card();
        Assert.IsFalse(cardUnderTest.IsFaceCard());

        cardUnderTest.Value = Values.ACE;
        Assert.IsFalse(cardUnderTest.IsFaceCard());
    }

    [Test]
    public void IsFaceCard_ForFaceCards_ReturnsTrue()
    {
        var cardUnderTest = new Card(Suits.CLUBS, Values.QUEEN);
        Assert.IsTrue(cardUnderTest.IsFaceCard());

        cardUnderTest.Value = Values.KING;
        Assert.IsTrue(cardUnderTest.IsFaceCard());

        cardUnderTest.Value = Values.JACK;
        Assert.IsTrue(cardUnderTest.IsFaceCard());
    }

    [Test]
    public void ToString_AceCard_ReturnsExpectedString()
    {
        var cardUnderTest = new Card();
        Assert.AreEqual(cardUnderTest.ToString(), "ace_of_clubs");
    }

    [Test]
    public void ToString_NumberCard_ReturnsExpectedString()
    {
        var cardUnderTest = new Card();
        cardUnderTest.Value = Values.EIGHT;
        Assert.AreEqual(cardUnderTest.ToString(), "8_of_clubs");
    }

    [Test]
    public void ToString_FaceCard_ReturnsExpectedString()
    {
        var cardUnderTest = new Card();
        cardUnderTest.Value = Values.JACK;
        cardUnderTest.Suit = Suits.DIAMONDS;
        Assert.AreEqual(cardUnderTest.ToString(), "jack_of_diamonds2");
    }

    [Test]
    public void GetTexturePath_Returns_ExpectedPath()
    {
        var cardUnderTest = new Card();
        Assert.AreEqual(cardUnderTest.GetTexturePath(), "Textures/ace_of_clubs");
    }

    [Test]
    public void GetSpritePath_Returns_ExpectedPath()
    {
        var cardUnderTest = new Card();
        cardUnderTest.Value = Values.JACK;
        cardUnderTest.Suit = Suits.SPADES;
        Assert.AreEqual(cardUnderTest.GetTexturePath(), "Textures/jack_of_spades2");
    }
}
