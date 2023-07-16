using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core.HandEvaluators
{
    public class PokerHandEvaluator : IHandEvaluator
    {
        const int MAX_HAND_SIZE = 7;
        /// <summary>
        /// Evaluates a 5-7 card hand for poker.
        /// Score is represented as a number composed up to 32 bits.
        /// h|t_1|...|t_n where
        /// h = the hand rank in decimal as binary.
        /// t_x = the tie breaker value to consider at tie x
        /// The hand given does not need to be at it's max length for the flavor of poker.
        /// This allows for evaluation at various steps in a poker game.
        /// </summary>
        public HandEvaluation Evaluate(IEnumerable<Card> hand)
        {
            var handList = hand.ToList();
            if (handList.Count > MAX_HAND_SIZE)
                throw new ArgumentException($"Cannot evalate a hand with more than {MAX_HAND_SIZE} cards.");

            (HandNames handName, bool acesHigh, IList<int> keyIndexes) = PokerHandDetector.Detect(handList);
            uint score = CalculateScore(handName, acesHigh, handList);
            return new HandEvaluation(handName, score, keyIndexes);
        }

        private static uint CalculateScore(HandNames handName, bool acesHigh, List<Card> hand)
        {
            uint ranking = HandNameToRank[handName];
            uint rankBits = RankScoreAsBits(ranking);
            uint tieBreakerBits = TieBreakerBits.Create(hand, handName, acesHigh);
            return rankBits | tieBreakerBits;
        }

        private static uint RankScoreAsBits(uint ranking)
        {
            return ranking << NibbleHelpers.GetLeftNibbleOffset(NUMBER_OF_BITS_IN_SCORE, 0);
        }

        private const int NUMBER_OF_BITS_IN_SCORE = (
            NibbleHelpers.BITS_PER_NIBBLE + TieBreakerBits.NUMBER_OF_BITS_IN_TIEBREAKER
        );

        private static readonly IReadOnlyDictionary<HandNames, uint> HandNameToRank = new Dictionary<HandNames, uint>() {
            { HandNames.NONE, 0 },
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

        private static class PokerHandDetector
        {
            public static (HandNames, bool, IList<int>) Detect(IEnumerable<Card> hand)
            {
                var handList = hand.ToList();
                if (handList.Count() == 0) return (HandNames.NONE, false, new List<int>());

                var (pairs, trips, quads, highCard, straight, acesHigh) = GetStats(handList);
                var suitGroups = handList.Select((_, i) => i)
                                         .GroupBy(i => handList[i].Suit)
                                         .OrderByDescending(x => x.Count());
                var groupWithMostFrequentSuit = suitGroups.First()
                                                          .OrderByDescending(i => GetValueAsByte(acesHigh, handList[i].Value))
                                                          .Take(NUMBER_OF_CARDS_IN_FLUSH)
                                                          .ToList();
                bool isFlush = groupWithMostFrequentSuit.Count() == NUMBER_OF_CARDS_IN_FLUSH;
                bool isStraight = straight.Count() == NUMBER_OF_CARDS_IN_STRAIGHT;
                bool isRoyalFlush = (
                    isStraight &&
                    isFlush &&
                    acesHigh &&
                    // Could have a hand with Ace not involved with straight.
                    // straight list is already ordered descending by hand value (aces high)
                    handList[straight[0]].Value == Values.ACE
                );

                if (isRoyalFlush)
                    return (HandNames.ROYAL_FLUSH, acesHigh, straight);
                if (isStraight && isFlush)
                    return (HandNames.STRAIGHT_FLUSH, acesHigh, straight);
                if (quads.Count() / 4 > 0)
                    return (HandNames.FOUR_OF_A_KIND, acesHigh, quads);
                if (pairs.Count() / 2 == 1 && trips.Count() / 3 == 1)
                    return (HandNames.FULL_HOUSE, acesHigh, pairs.Concat(trips).ToList());
                if (isFlush)
                    return (HandNames.FLUSH, acesHigh, groupWithMostFrequentSuit.ToList());
                if (isStraight)
                    return (HandNames.STRAIGHT, acesHigh, straight);
                if (trips.Count() / 3 > 0)
                    return (HandNames.THREE_OF_A_KIND, acesHigh, trips);
                if (pairs.Count() / 2 == 1)
                    return (HandNames.PAIR, acesHigh, pairs);
                if (pairs.Count() / 2 == 2)
                    return (HandNames.TWO_PAIR, acesHigh, pairs);
                return (HandNames.HIGH_CARD, acesHigh, new List<int>() { highCard });
            }

            private static (IList<int> pairs, IList<int> trips, IList<int> quads, int highCard, IList<int> straight, bool acesHigh) GetStats(IList<Card> hand)
            {
                var groupedByValue = hand.Select((_, i) => i)
                                         .GroupBy(i => GetValueAsByte(true, hand[i].Value))
                                         .OrderByDescending(x => x.Key);
                var frequencyStats = GetFrequencyStats(groupedByValue);

                return (
                    frequencyStats.pairs,
                    frequencyStats.trips,
                    frequencyStats.quads,
                    groupedByValue.First().First(),
                    frequencyStats.straight,
                    frequencyStats.acesHigh
                );
            }

            private static (
                List<int> pairs,
                List<int> trips,
                List<int> quads,
                List<int> straight,
                bool acesHigh
            ) GetFrequencyStats(IOrderedEnumerable<IGrouping<byte, int>> groupedByValue)
            {
                List<int> pairs = new();
                List<int> trips = new();
                List<int> quads = new();
                List<int> straight = new();
                bool acesLowInStraight = false;

                var groups = groupedByValue.ToList();
                bool hasAces = isAce(groups[0].Key);

                for (int i = 0; i < groupedByValue.Count() - 1; i++)
                {
                    var current = groups[i];
                    var next = groups[i + 1];
                    processGroup(current);

                    bool sequential = isSequential(current.Key, next.Key);
                    if (sequential) straight.Add(current.First());
                }
                processGroup(groups[^1]);

                if (hasAces && groups[^1].Key == (byte)Values.TWO)
                {
                    acesLowInStraight = true;
                    // Only add in the ACE if it does not already exist from comparing A to K
                    if (straight.Count > 0 && straight[0] != groups[0].First())
                        straight.Insert(0, groups[0].First());
                }

                if (straight.Count == NUMBER_OF_CARDS_IN_STRAIGHT - 1)
                    straight.Add(groups[^1].First());
                straight = straight.Take(NUMBER_OF_CARDS_IN_STRAIGHT).ToList();
                bool isLowAceStraight = acesLowInStraight && straight.Count == NUMBER_OF_CARDS_IN_STRAIGHT;

                void processGroup(IGrouping<byte, int> current)
                {
                    switch (current.Count())
                    {
                        case 2:
                            pairs.AddRange(current);
                            break;
                        case 3:
                            // You can't have two triples so keep the highest and move the
                            // low triple to the pair list as a pair.
                            if (trips.Count == 3)
                            {
                                var asPair = current.ToList().GetRange(0, 2);
                                pairs.AddRange(asPair);
                            }
                            else
                                trips.AddRange(current);
                            break;
                        case 4:
                            quads.AddRange(current);
                            break;
                    }
                }
                static bool isSequential(byte x, byte y) => x - y == 1;
                static bool isAce(byte x) => x == GetValueAsByte(true, Values.ACE);

                return (
                    pairs,
                    trips,
                    quads,
                    straight,
                    hasAces && !isLowAceStraight
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
                        hand.Select(x => GetValueAsByte(hasAcesHigh, x.Value))
                            .GroupBy(x => x)
                            .First().OrderByDescending(x => x),
                    HandNames.FLUSH or HandNames.STRAIGHT_FLUSH =>
                        hand.GroupBy(x => x.Suit)
                            .OrderByDescending(xs => xs.Count())
                            .First()
                            .Select(x => GetValueAsByte(hasAcesHigh, x.Value))
                            .OrderByDescending(x => x),
                    _ => hand.Select(x => GetValueAsByte(hasAcesHigh, x.Value))
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
        }

        private static class NibbleHelpers
        {
            public static int GetLeftNibbleOffset(int totalBitSize, int pos)
            {
                return totalBitSize - pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public static List<uint> ToList(uint nibbles)
            {
                List<uint> nibbleList = new();
                while (nibbles > 0)
                {
                    uint subNibble = nibbles & 0xF;
                    nibbleList.Insert(0, subNibble);
                    nibbles >>= BITS_PER_NIBBLE;
                }
                return nibbleList;
            }

            public const int BITS_PER_NIBBLE = 4;
        }

        private static byte GetValueAsByte(bool acesHigh, Values v)
        {
            if (acesHigh && v == Values.ACE)
                return (byte)(Values.KING + 1);
            return (byte)v;
        }
    }
}
