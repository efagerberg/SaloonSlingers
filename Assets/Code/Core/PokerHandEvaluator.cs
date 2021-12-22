using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class PokerHandEvaluator : IHandEvaluator
    {

        private enum HandTypes
        {
            HIGH_CARD = 1,
            PAIR,
            TWO_PAIR,
            THREE_OF_A_KIND,
            STRAIGHT,
            FLUSH,
            FULL_HOUSE,
            FOUR_OF_A_KIND,
            STRAIGHT_FLUSH,
            ROYAL_FLUSH
        }
        private const int HIGH_ACE_VALUE = (int)Values.KING + 1;
        private const int LOW_ACE_VALUE = 1;

        public float Evaluate(Card[] hand)
        {
            Lookups lookups = GetLookups(hand);
            Dictionary<HandTypes, float> handTypeToTieBreaker = new();
            CheckForOccurrenceBasedHands(handTypeToTieBreaker, lookups.ValuesByOccurrences);
            CheckForSequenceBasedHands(handTypeToTieBreaker, lookups.ValuesBySuits, lookups.SuitsByValues);
            CheckForHighCardHand(handTypeToTieBreaker, hand);
            return handTypeToTieBreaker.Sum(pair => (int)pair.Key + pair.Value);
        }

        private static Lookups GetLookups(Card[] hand)
        {
            Dictionary<Values, int> valueOccurences = new();
            Dictionary<Suits, HashSet<Values>> suitValues = new();
            Dictionary<Values, HashSet<Suits>> valueSuits = new();
            foreach (Card c in hand)
            {
                if (valueOccurences.ContainsKey(c.Value))
                    valueOccurences[c.Value] += 1;
                else
                    valueOccurences[c.Value] = 1;

                if (suitValues.ContainsKey(c.Suit))
                    suitValues[c.Suit].Add(c.Value);
                else
                    suitValues[c.Suit] = new HashSet<Values> { c.Value };

                if (valueSuits.ContainsKey(c.Value))
                    valueSuits[c.Value].Add(c.Suit);
                else
                    valueSuits[c.Value] = new HashSet<Suits> { c.Suit };
            }
            return new Lookups(valueOccurences, suitValues, valueSuits);
        }

        private static void CheckForOccurrenceBasedHands(Dictionary<HandTypes, float> handTypeToTieBreaker, Dictionary<Values, int> occurrences)
        {
            foreach (var pair in occurrences)
            {
                float tieBreakerValue = GetTieBreaker(pair.Key);
                if (pair.Value == 2)
                {
                    if (handTypeToTieBreaker.ContainsKey(HandTypes.PAIR))
                    {
                        var lastTieBreaker = handTypeToTieBreaker[HandTypes.PAIR];
                        handTypeToTieBreaker.Remove(HandTypes.PAIR);
                        handTypeToTieBreaker[HandTypes.TWO_PAIR] = lastTieBreaker + tieBreakerValue;
                    }
                    else handTypeToTieBreaker[HandTypes.PAIR] = tieBreakerValue;
                }
                else if (pair.Value == 3)
                    handTypeToTieBreaker[HandTypes.THREE_OF_A_KIND] = tieBreakerValue;
                else if (pair.Value == 4)
                    handTypeToTieBreaker[HandTypes.FOUR_OF_A_KIND] = tieBreakerValue;
            }

            if (handTypeToTieBreaker.ContainsKey(HandTypes.PAIR) && handTypeToTieBreaker.ContainsKey(HandTypes.THREE_OF_A_KIND))
            {
                float tieBreakerValue = handTypeToTieBreaker[HandTypes.THREE_OF_A_KIND];
                handTypeToTieBreaker.Remove(HandTypes.PAIR);
                handTypeToTieBreaker.Remove(HandTypes.THREE_OF_A_KIND);
                handTypeToTieBreaker[HandTypes.FULL_HOUSE] = tieBreakerValue;
            }
        }

        private static void CheckForSequenceBasedHands(Dictionary<HandTypes, float> handTypeToTieBreaker, Dictionary<Values, HashSet<Suits>> valuesBySuits, Dictionary<Suits, HashSet<Values>> suitsByValues)
        {
        }

        private static void CheckForHighCardHand(Dictionary<HandTypes, float> handTypeToTieBreaker, Card[] hand)
        {
            if (handTypeToTieBreaker.Count == 0 && hand.Length > 0)
            {
                Values? highestValueSeen = hand.Select(c => c.Value).OrderByDescending(v => v == Values.ACE ? int.MaxValue : (int)v).First();
                handTypeToTieBreaker[HandTypes.HIGH_CARD] = GetTieBreaker(highestValueSeen.Value);
            }
        }

        private struct Lookups
        {
            public readonly Dictionary<Values, int> ValuesByOccurrences;
            public readonly Dictionary<Suits, HashSet<Values>> SuitsByValues;
            public readonly Dictionary<Values, HashSet<Suits>> ValuesBySuits;

            public Lookups(
                Dictionary<Values, int> valuesByOccurrences,
                Dictionary<Suits, HashSet<Values>> suitsByValues,
                Dictionary<Values, HashSet<Suits>> valuesBySuits
            )
            {
                ValuesByOccurrences = valuesByOccurrences;
                SuitsByValues = suitsByValues;
                ValuesBySuits = valuesBySuits;
            }
        }

        public float GetMaxHandValue() => 10;
        public float GetMinHandValue() => 0;

        private static float GetTieBreaker(Values v) => GetPokerValue(v) / 100f;

        private static int GetPokerValue(Values v)
        {
            if (v == Values.ACE) return HIGH_ACE_VALUE;
            return (int)v;
        }
    }
}
