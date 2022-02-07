using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core.Tests
{
    public class HandTests
    {
        public class TestAdd
        {
            [TestCase("", "AH")]
            [TestCase("AH", "AH")]
            [TestCase("AH 2D", "3H")]
            public void TestAdd_AddsCardAndUpdatesHandType(string handString, string newCardString)
            {
                TestHandEvaluator evaluator = new();
                Hand subject = new(evaluator, TestHelpers.MakeHandFromString(handString));
                Card newCard = new(newCardString);
                IEnumerable<Card> expectedCards = TestHelpers.MakeHandFromString(handString)
                                                             .Append(newCard);
                HandType expectedHandType = evaluator.Evaluate(expectedCards);

                subject.Add(newCard);

                Assert.AreEqual(expectedCards, subject.Cards);
                Assert.AreEqual(expectedHandType, subject.HandType);
            }
        }

        public class TestRemove
        {
            [TestCase("AH", 0)]
            [TestCase("AH 2D", 1)]
            [TestCase("", 0)]
            public void TestRemove_RemovesCardAndUpdatesHandType(string handString, int index)
            {
                TestHandEvaluator evaluator = new();
                Hand subject = new(evaluator, TestHelpers.MakeHandFromString(handString));
                IEnumerable<Card> expectedCards = TestHelpers.MakeHandFromString(handString)
                                                             .Where((card, i) => i != index);
                HandType expectedHandType = evaluator.Evaluate(expectedCards);

                subject.Remove(index);

                Assert.AreEqual(expectedCards, subject.Cards);
                Assert.AreEqual(expectedHandType, subject.HandType);
            }
        }

        private class TestHandEvaluator : IHandEvaluator
        {
            public HandType Evaluate(IEnumerable<Card> hand)
            {
                int score = hand.Count();
                return new HandType((HandNames)score, (uint)score);
            }
        }
    }
}
