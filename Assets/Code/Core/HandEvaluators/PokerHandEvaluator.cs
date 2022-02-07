using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SaloonSlingers.Core.HandEvaluators
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
        public HandType Evaluate(IEnumerable<Card> hand)
        {
            var handList = hand.ToList();
            (HandNames handName, bool hasAcesHigh) = HandTypeDetector.Detect(handList);
            return CreateHandType(
                CalculateScore(handName, hasAcesHigh, handList),
                handName
            );
        }

        private static uint CalculateScore(HandNames handName, bool hasAcesHigh, List<Card> hand)
        {
            uint ranking = HandNameToRank[handName];
            uint handTypeBits = RankScoreAsBits(ranking);
            uint tieBreakerBits = TieBreakerBits.Create(hand, handName, hasAcesHigh);
            return handTypeBits | tieBreakerBits;
        }

        private static uint RankScoreAsBits(uint ranking)
        {
            return ranking << NibbleHelpers.GetLeftNibbleOffset(NUMBER_OF_BITS_IN_SCORE, 0);
        }

        private const int NUMBER_OF_BITS_IN_SCORE = (
            NibbleHelpers.BITS_PER_NIBBLE + TieBreakerBits.NUMBER_OF_BITS_IN_TIEBREAKER
        );

        private static IReadOnlyDictionary<HandNames, uint> HandNameToRank = new Dictionary<HandNames, uint>() {
            { HandNames.EMPTY, 0 },
            { HandNames.HIGH_CARD, 1 },
            { HandNames.PAIR, 2 },
            { HandNames.TWO_PAIR, 3 },
            { HandNames.THREE_OF_A_KIND, 4 },
            { HandNames.STRAIGHT, 5 },
            { HandNames.FLUSH, 6 },
            { HandNames.FULL_HOUSE, 7 },
            { HandNames.FOUR_OF_A_KIND, 8 },
            { HandNames.STRAIGHT_FLUSH, 9 },
            { HandNames.ROYAL_FLUSH, 10 }
        };

        private static class HandTypeDetector
        {
            public static (HandNames, bool) Detect(IEnumerable<Card> hand)
            {
                var handList = hand.ToList();
                if (handList.Count() == 0) return (HandNames.EMPTY, false);

                var (nPairs, nTrips, nQuads, isStraight, hasAcesHigh) = GetFrequencyStats(handList);
                var suitPairs = handList.GroupBy(x => x.Suit).OrderByDescending(x => x.Count());
                var groupWithMostFrequentSuit = suitPairs.First();
                bool isFlush = groupWithMostFrequentSuit.Count() == NUMBER_OF_CARDS_IN_FLUSH;
                bool isRoyalFlush = (
                    groupWithMostFrequentSuit.Select(x => x.Value).Contains(Values.ACE) &&
                    isStraight && isFlush
                );

                if (isRoyalFlush) return (HandNames.ROYAL_FLUSH, hasAcesHigh);
                if (isStraight && isFlush) return (HandNames.STRAIGHT_FLUSH, hasAcesHigh);
                if (nQuads > 0) return (HandNames.FOUR_OF_A_KIND, hasAcesHigh);
                if (nPairs == 1 && nTrips == 1) return (HandNames.FULL_HOUSE, hasAcesHigh);
                if (isFlush) return (HandNames.FLUSH, hasAcesHigh);
                if (isStraight) return (HandNames.STRAIGHT, hasAcesHigh);
                if (nTrips > 0) return (HandNames.THREE_OF_A_KIND, hasAcesHigh);
                if (nPairs == 1) return (HandNames.PAIR, hasAcesHigh);
                if (nPairs == 2) return (HandNames.TWO_PAIR, hasAcesHigh);
                return (HandNames.HIGH_CARD, hasAcesHigh);
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
            private const int NUMBER_OF_CARDS_IN_STRAIGHT = 5;
            private const int NUMBER_OF_CARDS_IN_FLUSH = 5;
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
            public const int NUMBER_OF_BITS_IN_TIEBREAKER = (
                MAX_NUMBER_OF_TIEBREAKERS * NibbleHelpers.BITS_PER_NIBBLE
            );

            public static uint Create(IEnumerable<Card> hand, HandNames handName, bool hasAcesHigh)
            {
                int tieBreakerIndex = 0;
                uint tieBreakers = 0;
                var valuesOrdered = GetValuesToConsider(hand, handName, hasAcesHigh);
                foreach (byte cardValue in valuesOrdered)
                {
                    tieBreakers |= CalculateTieBreaker(cardValue, tieBreakerIndex);
                    tieBreakerIndex += 1;
                }

                return tieBreakers;
            }

            private static IEnumerable<byte> GetValuesToConsider(IEnumerable<Card> hand, HandNames handName, bool hasAcesHigh)
            {
                int numberOfValuesToConsider = handName switch
                {
                    HandNames.PAIR => MAX_NUMBER_OF_TIEBREAKERS - 1,
                    HandNames.TWO_PAIR => MAX_NUMBER_OF_TIEBREAKERS - 2,
                    HandNames.THREE_OF_A_KIND => MAX_NUMBER_OF_TIEBREAKERS - 2,
                    HandNames.FOUR_OF_A_KIND => MAX_NUMBER_OF_TIEBREAKERS - 3,
                    HandNames.STRAIGHT or HandNames.STRAIGHT_FLUSH => MAX_NUMBER_OF_TIEBREAKERS - 4,
                    HandNames.FULL_HOUSE => MAX_NUMBER_OF_TIEBREAKERS - 3,
                    HandNames.ROYAL_FLUSH => 0,
                    _ => MAX_NUMBER_OF_TIEBREAKERS
                };

                IEnumerable<byte> values = handName switch
                {
                    HandNames.STRAIGHT =>
                        hand.Select(x => GetByteValues(hasAcesHigh, x))
                            .GroupBy(x => x)
                            .First().OrderByDescending(x => x),
                    HandNames.FLUSH or HandNames.STRAIGHT_FLUSH =>
                        hand.GroupBy(x => x.Suit)
                            .OrderByDescending(xs => xs.Count())
                            .First()
                            .Select(x => GetByteValues(hasAcesHigh, x))
                            .OrderByDescending(x => x),
                    _ => hand.Select(x => GetByteValues(hasAcesHigh, x))
                             .GroupBy(x => x)
                             .OrderByDescending(xs => xs.Count())
                             .ThenByDescending(xs => xs.Key)
                             .Select(xs => xs.Key),
                };

                return values.Take(numberOfValuesToConsider);
            }

            private static byte GetByteValues(bool hasAcesHigh, Card x)
            {
                if (hasAcesHigh && x.Value == Values.ACE)
                    return (byte)(Values.KING + 1);
                return (byte)x.Value;
            }

            private static uint CalculateTieBreaker(byte cardValue, int tieBreakerIndex)
            {
                return (uint)(cardValue << NibbleHelpers.GetLeftNibbleOffset(
                    NUMBER_OF_BITS_IN_TIEBREAKER, tieBreakerIndex
                ));
            }

            private const int MAX_NUMBER_OF_TIEBREAKERS = 5;
        }

        private static class NibbleHelpers
        {
            public static int GetLeftNibbleOffset(int totalBitSize, int pos)
            {
                return totalBitSize - pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public const int BITS_PER_NIBBLE = 4;
        }

        private HandType CreateHandType(uint score, HandNames handName)
        {
            string enumName = Enum.GetName(typeof(HandNames), handName);
            TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return new HandType(handName, score);
        }
    }
}
