using System;
using System.Collections.Generic;
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

            IEnumerable<Values> expectedValues = Enum.GetValues(typeof(Values)).Cast<Values>();
            IEnumerable<Suits> expectedSuits = Enum.GetValues(typeof(Suits)).Cast<Suits>();
            List<Card> cards = new();

            for (int _ = 0; _ < deckUnderTest.Count; _++)
                cards.Add(deckUnderTest.Draw());

            foreach (Suits suit in expectedSuits)
                foreach (Values val in expectedValues)
                    cards.Remove(new Card(val, suit));

            Assert.IsEmpty(cards);
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
        public void Draw_NoParam_ReturnsExpectedCard()
        {
            var expectedCount = deckUnderTest.Count - 1;
            deckUnderTest.Draw();

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }

        [Test]
        public void Draw_EmitsEmptyEvent_WhenDeckBecomesEmpty()
        {
            var singleCardDeck = new Deck(1);
            bool eventTriggered = false;
            singleCardDeck.Emptied += (_, __) => eventTriggered = true;
            singleCardDeck.Draw();

            Assert.That(eventTriggered);
        }

        [Test]
        public void Draw_DoesNotEmitEmptyEvent_WhenDeckDoesNotBecomeEmpty()
        {
            var twoCardDeck = new Deck(2);
            bool eventTriggered = false;
            twoCardDeck.Emptied += (_, __) => eventTriggered = true;
            twoCardDeck.Draw();

            Assert.IsFalse(eventTriggered);
        }

        [Test]
        public void Draw_WithAmount_ReturnExpectedCards()
        {
            var amountToTake = 7;
            var expectedCount = deckUnderTest.Count - amountToTake;
            var cards = deckUnderTest.Draw(amountToTake).ToList();

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
            Assert.AreEqual(cards.ToList().Count, amountToTake);
        }

        [Test]
        public void Return_ReturnsCardToDeck()
        {
            var expectedCount = deckUnderTest.Count;
            var card = deckUnderTest.Draw();
            deckUnderTest.Return(card);

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }

        [Test]
        public void Return_EmitsRefillEvent_WhenSizeIncreasesFrom0()
        {
            bool eventTriggered = false;
            var emptyDeck = new Deck(0);
            emptyDeck.Refilled += (sender, __) => eventTriggered = true;
            emptyDeck.Return(new Card(Values.ACE, Suits.CLUBS));

            Assert.That(eventTriggered);
        }

        [Test]
        public void Return_DoesNotEmitsRefillEvent_WhenSizeAbove0()
        {
            bool eventTriggered = false;
            var singleCardDeck = new Deck(1);
            singleCardDeck.Refilled += (sender, __) => eventTriggered = true;
            singleCardDeck.Return(new Card(Values.ACE, Suits.CLUBS));

            Assert.IsFalse(eventTriggered);
        }

        [Test]
        public void Return_ReturnsCardsToDeck()
        {
            var expectedCount = deckUnderTest.Count;
            var cards = deckUnderTest.Draw(7);
            deckUnderTest.Return(cards);

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }
    }

}
