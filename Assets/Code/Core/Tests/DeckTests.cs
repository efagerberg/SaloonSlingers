using System;
using System.Linq;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
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
        public void Default_Constructor_ReturnsExpectedDeck()
        {
            Assert.AreEqual(deckUnderTest.Count, Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK);

            var expectedValues = Enum.GetValues(typeof(Values)).Cast<Values>();
            var expectedSuits = Enum.GetValues(typeof(Suits)).Cast<Suits>();

            foreach (var suit in expectedSuits)
            {
                foreach (var val in expectedValues)
                {
                    var cards = deckUnderTest.Where(x => x.Suit == suit && x.Value == val);
                    Assert.AreEqual(cards.Count(), 1);
                }
            }
        }

        [TestCase(1)]
        [TestCase(20)]
        [TestCase(-1)]
        [TestCase(Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK * 3)]
        public void Constructor_WithNumberOfCards_ReturnsExpectedDeck(int numberOfCards)
        {
            var deck = new Deck(numberOfCards);

            Assert.AreEqual(deck.Count, Math.Max(0, numberOfCards));
        }

        [Test]
        public void RemoveFromTop_NoParam_ReturnsExpectedCard()
        {
            var expectedCount = deckUnderTest.Count - 1;
            deckUnderTest.RemoveFromTop();

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }

        [Test]
        public void RemoveFromTop_WithAmount_ReturnExpectedCards()
        {
            var amountToTake = 7;
            var expectedCount = deckUnderTest.Count - amountToTake;
            var cards = deckUnderTest.RemoveFromTop(amountToTake).ToList();

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
            Assert.AreEqual(cards.ToList().Count, amountToTake);
        }

        [Test]
        public void ReturnCard_ReturnsCardToDeck()
        {
            var expectedCount = deckUnderTest.Count;
            var card = deckUnderTest.RemoveFromTop();
            deckUnderTest.ReturnCard(card);

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }

        [Test]
        public void ReturnCards_ReturnsCardsToDeck()
        {
            var expectedCount = deckUnderTest.Count;
            var cards = deckUnderTest.RemoveFromTop(7);
            deckUnderTest.ReturnCards(cards);

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }
    }

}
