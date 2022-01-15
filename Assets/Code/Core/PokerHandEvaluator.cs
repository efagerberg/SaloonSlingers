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
            HandTypes handType = HandTypeDetector.Detect(handList);
            return CalculateScore(handType, handList);
        }

        private static uint CalculateScore(HandTypes handType, List<Card> hand)
        {
            uint handTypeBits = HandTypeScoreAsBits(handType);
            uint tieBreakerBits = TieBreakerBits.Create(hand, handType);
            return handTypeBits | tieBreakerBits;
        }

        private static uint HandTypeScoreAsBits(HandTypes handType)
        {
            return (uint)handType << NibbleHelpers.GetLeftNibbleOffset(NUMBER_OF_BITS_IN_SCORE, 0);
        }

        private const int NUMBER_OF_BITS_IN_SCORE = 32;

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
            public static HandTypes Detect(IEnumerable<Card> hand)
            {
                int numCardsInStraight = 5;
                int numCardsInFlush = 5;
                var handList = hand.ToList();
                var (nPairs, nTrips, nQuads, numInSequence) = GetFrequencyStats(handList);
                var suitPairs = handList.GroupBy(x => x.Suit).OrderByDescending(x => x.Count());
                var groupWithMostFrequentSuit = suitPairs.First();
                bool isFlush = groupWithMostFrequentSuit.Count() == numCardsInFlush;
                bool isStraight = numInSequence == numCardsInStraight;
                bool isRoyalFlush = (
                    groupWithMostFrequentSuit.Select(x => x.Value).Contains(Values.ACE) &&
                    isStraight && isFlush
                );

                if (isRoyalFlush) return HandTypes.ROYAL_FLUSH;
                if (isStraight && isFlush) return HandTypes.STRAIGHT_FLUSH;
                if (nQuads > 0) return HandTypes.FOUR_OF_A_KIND;
                if (nPairs == 1 && nTrips == 1) return HandTypes.FULL_HOUSE;
                if (isFlush) return HandTypes.FLUSH;
                if (isStraight) return HandTypes.STRAIGHT;
                if (nTrips > 0) return HandTypes.THREE_OF_A_KIND;
                if (nPairs == 1) return HandTypes.PAIR;
                if (nPairs == 2) return HandTypes.TWO_PAIR;
                return HandTypes.HIGH_CARD;
            }

            private static (int nPairs, int nTrips, int nQuads, int numInSequence) GetFrequencyStats(IEnumerable<Card> hand)
            {
                var pairs = hand.GroupBy(x => (byte)x.Value).OrderByDescending(x => x.Key);
                var initialPairAcc = (nPairs: 0, nTrips: 0, nQuads: 0, maxValue: (byte)0, lastValue: (byte)0, numInSequence: 1);
                var stats = pairs.Aggregate(initialPairAcc, (acc, pair) =>
                {
                    if (acc.lastValue != 0 && acc.maxValue != 0)
                    {
                        bool isInSequence = (
                            acc.lastValue - pair.Key == 1 ||
                            acc.maxValue - pair.Key == (int)Values.KING - (int)Values.ACE
                        );
                        if (isInSequence) acc.numInSequence += 1;
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
                return (stats.nPairs, stats.nTrips, stats.nQuads, stats.numInSequence);
            }
        }

        /// <summary>
        /// Tiebreaker schema: t_1|t_2|t_3|...|t_n
        /// t_x = the tie breaker value to consider at tie x as a nibble
        /// A high card tie breaker for AH 9C 3S 5C 2D would be
        /// 1110 1001 0101 0011 0010
        /// A tie breaker for a pair hand 7H 7C 3S 5C 2D would be
        /// 0111 0101 0011 0010 0000
        /// </summary>
        private static class TieBreakerBits
        {
            public static uint Create(IEnumerable<Card> hand, HandTypes handType)
            {
                int tieBreakerIndex = 0;
                uint tieBreakers = 0;
                var valuesOrdered = GetValuesOrdered(hand, handType);
                foreach (byte cardValue in valuesOrdered)
                {
                    tieBreakers |= CalculateTieBreaker(cardValue, tieBreakerIndex);
                    tieBreakerIndex += 1;
                }

                return tieBreakers;
            }

            private static IEnumerable<byte> GetValuesOrdered(IEnumerable<Card> hand, HandTypes handType)
            {
                static byte withAcesHigh(Card x)
                {
                    return x.Value == Values.ACE ? (byte)(Values.KING + 1) : (byte)x.Value;
                }

                return handType switch
                {
                    HandTypes.FLUSH or HandTypes.STRAIGHT_FLUSH =>
                        hand.GroupBy(x => x.Suit)
                            .OrderByDescending(xs => xs.Count())
                            .Select(xs => xs.OrderByDescending(x => x.Value))
                            .SelectMany((xs, i) =>
                            {
                                var values = xs.Select(x => i > 0 ? withAcesHigh(x) : (byte)x.Value);
                                return values.OrderByDescending(x => x);
                            }),
                    HandTypes.ROYAL_FLUSH =>
                        hand.GroupBy(x => x.Suit)
                            .OrderByDescending(xs => xs.Count())
                            .Select(xs => xs.OrderByDescending(x => x.Value))
                            .SelectMany(xs => xs.Select(withAcesHigh).OrderByDescending(x => x)),
                    _ => hand.Select(withAcesHigh)
                             .GroupBy(x => x)
                             .OrderByDescending(xs => xs.Count())
                             .ThenByDescending(xs => xs.Key)
                             .Select(xs => xs.Key),
                };
            }

            internal static uint CalculateTieBreaker(byte cardValue, int tieBreakerIndex)
            {
                return (uint)(cardValue << NibbleHelpers.GetLeftNibbleOffset(
                    NUMBER_OF_BITS_IN_TIEBREAKER, tieBreakerIndex
                ));
            }

            private const int NUMBER_OF_BITS_IN_TIEBREAKER = 28;
        }

        private static class NibbleHelpers
        {
            public static int GetRightNibbleOffset(int pos)
            {
                return pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public static int GetLeftNibbleOffset(int totalBitSize, int pos)
            {
                return totalBitSize - pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public const int BITS_PER_NIBBLE = 4;
        }
    }
}
