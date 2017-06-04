using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

public class DeckTests
{
    Deck deckUnderTest;

    [SetUp]
    public void Init()
    {
        deckUnderTest = new Deck();
        deckUnderTest.Shuffle();
    }

    [TearDown]
    public void Dispose()
    {
        deckUnderTest = null;
    }

    [Test]
    public void Constructor_ReturnsExpectedDeck()
    {
        Assert.AreEqual(deckUnderTest.Count, Constants.DEFAULT_DECK_SIZE);

        var expectedValues = Enum.GetValues(typeof(Values)).Cast<Values>();
        var expectedSuits = Enum.GetValues(typeof(Suits)).Cast<Suits>();

        foreach (var suit in expectedSuits)
        {
            foreach (var val in expectedValues)
            {
                var cards = deckUnderTest.Where(x => x.Suit == suit &&
                                                      x.Value == val).Select(x => x);
                Assert.AreEqual(cards.Count(), 1);
            }
        }
    }

    [Test]
    public void RemoveFromTop_DefaultParam_ReturnsExpectedCard()
    {
        deckUnderTest.RemoveFromTop();
        Assert.AreEqual(deckUnderTest.Count, Constants.DEFAULT_DECK_SIZE - 1);
    }

    [Test]
    public void RemoveFromTop_Cards_ReturnExpectedCards()
    {
        var cards = new List<Card>();
        deckUnderTest.RemoveFromTop(7, cards);
        Assert.AreEqual(deckUnderTest.Count, Constants.DEFAULT_DECK_SIZE - 7);
        Assert.AreEqual(cards.Count, 7);
    }

    [Test]
    public void ReturnCard_ReturnsCardToDeck()
    {
        var card = deckUnderTest.RemoveFromTop();
        deckUnderTest.ReturnCard(card);
        Assert.AreEqual(deckUnderTest.Count, Constants.DEFAULT_DECK_SIZE);
    }

    [Test]
    public void ReturnCards_ReturnsCardsToDeck()
    {
        var cards = new List<Card>();
        deckUnderTest.RemoveFromTop(7, cards);
        deckUnderTest.ReturnCards(cards);
        Assert.AreEqual(deckUnderTest.Count, Constants.DEFAULT_DECK_SIZE);
    }
}

