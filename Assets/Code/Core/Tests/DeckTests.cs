﻿using System;
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
                cards.Add(deckUnderTest.RemoveFromTop());

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
        public void RemoveFromTop_NoParam_ReturnsExpectedCard()
        {
            var expectedCount = deckUnderTest.Count - 1;
            deckUnderTest.RemoveFromTop();

            Assert.AreEqual(deckUnderTest.Count, expectedCount);
        }

        [Test]
        public void RemoveFromTop_EmitsEmptyEvent_WhenDeckBecomesEmpty()
        {
            var singleCardDeck = new Deck(1);
            bool eventTriggered = false;
            singleCardDeck.OnDeckEmpty += (_, __) => eventTriggered = true;
            singleCardDeck.RemoveFromTop();

            Assert.That(eventTriggered);
        }

        [Test]
        public void RemoveFromTop_DoesNotEmitEmptyEvent_WhenDeckDoesNotBecomeEmpty()
        {
            var twoCardDeck = new Deck(2);
            bool eventTriggered = false;
            twoCardDeck.OnDeckEmpty += (_, __) => eventTriggered = true;
            twoCardDeck.RemoveFromTop();

            Assert.IsFalse(eventTriggered);
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
        public void ReturnCard_EmitsRefillEvent_WhenSizeIncreasesFrom0()
        {
            bool eventTriggered = false;
            var emptyDeck = new Deck(0);
            emptyDeck.OnDeckRefilled += (sender, __) => eventTriggered = true;
            emptyDeck.ReturnCard(new Card(Values.ACE, Suits.CLUBS));

            Assert.That(eventTriggered);
        }

        [Test]
        public void ReturnCard_DoesNotEmitsRefillEvent_WhenSizeAbove0()
        {
            bool eventTriggered = false;
            var singleCardDeck = new Deck(1);
            singleCardDeck.OnDeckRefilled += (sender, __) => eventTriggered = true;
            singleCardDeck.ReturnCard(new Card(Values.ACE, Suits.CLUBS));

            Assert.IsFalse(eventTriggered);
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
