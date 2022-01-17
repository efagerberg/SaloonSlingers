using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class PokerHandEvaluator : IHandEvaluator
    {
        /// <summary>
        /// Evaluates a 5-7 card hand for poker.
        /// Score is represented as a number composed up to 32 bits.
        /// h|t_1|...|t_n where
        /// h = the hand rank in decimal as binary.
        /// t_x = the tie breaker value to consider at tie x
        /// The hand given does not need to be at it's max length for the flavor of poker.
        /// This allows for evaluation at various steps in a poker game.
        /// </summary>
        public uint Evaluate(IEnumerable<Card> hand)
        {
            var handList = hand.ToList();
            if (handList.Count() == 0) return (int)HandTypes.HIGH_CARD - 1;
            (HandTypes handType, bool hasAcesHigh) = HandTypeDetector.Detect(handList);
            return CalculateScore(handType, hasAcesHigh, handList);
        }

        private static uint CalculateScore(HandTypes handType, bool hasAcesHigh, List<Card> hand)
        {
            uint handTypeBits = HandTypeScoreAsBits(handType);
            uint tieBreakerBits = TieBreakerBits.Create(hand, handType, hasAcesHigh);
            return handTypeBits | tieBreakerBits;
        }

        private static uint HandTypeScoreAsBits(HandTypes handType)
        {
            return (uint)handType << NibbleHelpers.GetLeftNibbleOffset(NUMBER_OF_BITS_IN_SCORE, 0);
        }

        private const int NUMBER_OF_BITS_IN_SCORE = (
            NibbleHelpers.BITS_PER_NIBBLE + TieBreakerBits.NUMBER_OF_BITS_IN_TIEBREAKER
        );

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

        private static class HandTypeDetector
        {
            private const int NUMBER_OF_CARDS_IN_STRAIGHT = 5;

            public static (HandTypes, bool) Detect(IEnumerable<Card> hand)
            {
                int numCardsInFlush = 5;
                var handList = hand.ToList();
                var (nPairs, nTrips, nQuads, isStraight, hasAcesHigh) = GetFrequencyStats(handList);
                var suitPairs = handList.GroupBy(x => x.Suit).OrderByDescending(x => x.Count());
                var groupWithMostFrequentSuit = suitPairs.First();
                bool isFlush = groupWithMostFrequentSuit.Count() == numCardsInFlush;
                bool isRoyalFlush = (
                    groupWithMostFrequentSuit.Select(x => x.Value).Contains(Values.ACE) &&
                    isStraight && isFlush
                );

                if (isRoyalFlush) return (HandTypes.ROYAL_FLUSH, hasAcesHigh);
                if (isStraight && isFlush) return (HandTypes.STRAIGHT_FLUSH, hasAcesHigh);
                if (nQuads > 0) return (HandTypes.FOUR_OF_A_KIND, hasAcesHigh);
                if (nPairs == 1 && nTrips == 1) return (HandTypes.FULL_HOUSE, hasAcesHigh);
                if (isFlush) return (HandTypes.FLUSH, hasAcesHigh);
                if (isStraight) return (HandTypes.STRAIGHT, hasAcesHigh);
                if (nTrips > 0) return (HandTypes.THREE_OF_A_KIND, hasAcesHigh);
                if (nPairs == 1) return (HandTypes.PAIR, hasAcesHigh);
                if (nPairs == 2) return (HandTypes.TWO_PAIR, hasAcesHigh);
                return (HandTypes.HIGH_CARD, hasAcesHigh);
            }

            private static (int nPairs, int nTrips, int nQuads, bool isStraight, bool hasAcesHigh) GetFrequencyStats(IEnumerable<Card> hand)
            {
                int numCardsInStraight = NUMBER_OF_CARDS_IN_STRAIGHT;
                var pairs = hand.GroupBy(x => (byte)x.Value).OrderByDescending(x => x.Key);
                var initialPairAcc = (
                    nPairs: 0,
                    nTrips: 0,
                    nQuads: 0,
                    maxValue: (byte)0,
                    lastValue: (byte)0,
                    numInSequence: 1,
                    hasHighAceInSequence: false
                );
                var stats = pairs.Aggregate(initialPairAcc, (acc, pair) =>
                {
                    if (acc.lastValue != 0 && acc.maxValue != 0)
                    {
                        bool isHighAceSequence = acc.maxValue - pair.Key == (int)Values.KING - (int)Values.ACE;
                        if (isHighAceSequence)
                        {
                            acc.numInSequence += 1;
                            acc.hasHighAceInSequence = true;
                        }
                        else if (acc.lastValue - pair.Key == 1)
                        {
                            acc.numInSequence += 1;
                        }
                    }

                    switch (pair.Count())
                    {
                        case 2:
                            acc.nPairs += 1;
                            break;
                        case 3:
                            acc.nTrips += 1;
                            break;
                        case 4:
                            acc.nQuads += 1;
                            break;
                    }
                    acc.lastValue = pair.Key;
                    if (acc.maxValue == 0) acc.maxValue = pair.Key;
                    return acc;
                });

                bool isStraight = stats.numInSequence == numCardsInStraight;

                return (
                    stats.nPairs,
                    stats.nTrips,
                    stats.nQuads,
                    isStraight,
                    !isStraight || stats.hasHighAceInSequence
                );
            }
        }

        /// <summary>
        /// Tiebreaker schema: t_1|t_2|t_3|...|t_n
        /// t_x = the tie breaker value to consider at tie x as a nibble
        /// A high card tie breaker for AH 9C 3S 5C 2D would be
        /// 1110 1001 0101 0011 0010
        /// A tie breaker for a pair hand 7H 7C 3S 5C 2D would be
        /// 0111 0101 0011 0010 0000
        /// We assume there are a max of 5 cards to consider for breaking ties.
        /// </summary>
        private static class TieBreakerBits
        {
            public static uint Create(IEnumerable<Card> hand, HandTypes handType, bool hasAcesHigh)
            {
                int tieBreakerIndex = 0;
                uint tieBreakers = 0;
                var valuesOrdered = GetValuesToConsider(hand, handType, hasAcesHigh);
                foreach (byte cardValue in valuesOrdered)
                {
                    tieBreakers |= CalculateTieBreaker(cardValue, tieBreakerIndex);
                    tieBreakerIndex += 1;
                }

                return tieBreakers;
            }

            private static IEnumerable<byte> GetValuesToConsider(IEnumerable<Card> hand, HandTypes handType, bool hasAcesHigh)
            {
                static byte byteValues(bool hasAcesHigh, Card x)
                {
                    if (hasAcesHigh && x.Value == Values.ACE)
                        return (byte)(Values.KING + 1);
                    return (byte)x.Value;
                }

                int numberOfValuesToConsider = handType switch
                {
                    HandTypes.PAIR => MAX_NUMBER_OF_TIEBREAKERS - 1,
                    HandTypes.TWO_PAIR => MAX_NUMBER_OF_TIEBREAKERS - 2,
                    HandTypes.THREE_OF_A_KIND => MAX_NUMBER_OF_TIEBREAKERS - 2,
                    HandTypes.FOUR_OF_A_KIND => MAX_NUMBER_OF_TIEBREAKERS - 3,
                    HandTypes.FULL_HOUSE => MAX_NUMBER_OF_TIEBREAKERS - 3,
                    HandTypes.STRAIGHT or HandTypes.STRAIGHT_FLUSH => MAX_NUMBER_OF_TIEBREAKERS - 4,
                    HandTypes.ROYAL_FLUSH => 0,
                    _ => MAX_NUMBER_OF_TIEBREAKERS
                };

                var values = handType switch
                {
                    HandTypes.STRAIGHT =>
                        hand.Select(x => byteValues(hasAcesHigh, x))
                            .GroupBy(x => x)
                            .First().OrderByDescending(x => x),
                    HandTypes.FLUSH or HandTypes.STRAIGHT_FLUSH =>
                        hand.GroupBy(x => x.Suit)
                            .OrderByDescending(xs => xs.Count())
                            .First()
                            .Select(x => byteValues(hasAcesHigh, x))
                            .OrderByDescending(x => x),
                    _ => hand.Select(x => byteValues(hasAcesHigh, x))
                             .GroupBy(x => x)
                             .OrderByDescending(xs => xs.Count())
                             .ThenByDescending(xs => xs.Key)
                             .Select(xs => xs.Key),
                };
                return values.Take(numberOfValuesToConsider);
            }

            private static uint CalculateTieBreaker(byte cardValue, int tieBreakerIndex)
            {
                return (uint)(cardValue << NibbleHelpers.GetLeftNibbleOffset(
                    NUMBER_OF_BITS_IN_TIEBREAKER, tieBreakerIndex
                ));
            }

            private const int MAX_NUMBER_OF_TIEBREAKERS = 5;
            public const int NUMBER_OF_BITS_IN_TIEBREAKER = (
                MAX_NUMBER_OF_TIEBREAKERS * NibbleHelpers.BITS_PER_NIBBLE
            );
        }

        private static class NibbleHelpers
        {
            public static int GetLeftNibbleOffset(int totalBitSize, int pos)
            {
                return totalBitSize - pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public const int BITS_PER_NIBBLE = 4;
        }
    }
}
