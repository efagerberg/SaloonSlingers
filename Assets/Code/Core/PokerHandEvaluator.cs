using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public class PokerHandEvaluator : IHandEvaluator
    {
        /// <summary>
        /// Evaluates a hand for poker.
        /// Score is represented as a number composed up to 32 bits.
        /// h|t_1|...|t_n where
        /// h = the hand rank in decimal as binary.
        /// t_x = the tie breaker value to consider at tie x
        /// The hand given does not need to be at it's max length for the flavor of poker.
        /// This allows for evaluation at various steps in a poker game.
        /// </summary>
        public int Evaluate(IEnumerable<Card> hand)
        {
            ulong handBits = HandBits.Create(hand);
            if (handBits == 0) return (int)HandTypes.HIGH_CARD - 1;
            return CalculateScore(HandTypes.HIGH_CARD, handBits);
        }

        private static int CalculateScore(HandTypes handType, ulong handBits)
        {
            int handTypeBits = HandBits.CalculateHandTypeScoreBits(handType);
            int tieBreakerBits = TieBreakerBits.Create(handBits, true);
            return handTypeBits | tieBreakerBits;
        }

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

        /// <summary>
        /// Representation of hand as bits
        /// 
        /// Each card is a nybble in the format: cdhs
        /// So a hand with a King of Hearts and Queen of Clubs would be:
        /// 0010|1000|0000|....
        /// This allows you to encode the count of each type of card and their values is set positionally.
        /// so that you can easily use bitwise shifts to see if they are consecutive.
        /// </summary>
        private static class HandBits
        {
            public static ulong Create(IEnumerable<Card> hand)
            {
                ulong x = 0;
                foreach (Card card in hand)
                    x |= (ulong)Card.GetSuitMask(card) << (4 * (int)card.Value) - 4;
                return x;
            }

            public static int CalculateHandTypeScoreBits(HandTypes handType)
            {
                return (int)handType << NibbleHelpers.ConvertLeftNibbleIndexToOffset(NUMBER_OF_BITS_IN_SCORE, 0);
            }

            private const int NUMBER_OF_BITS_IN_SCORE = 32;
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
            public static int Create(ulong handBits, bool acesHigh)
            {
                int tieBreakerIndex = 0;
                int tieBreakers = 0;
                for (byte cardValue = (byte)Values.KING; cardValue >= (byte)Values.ACE; cardValue -= 1)
                {
                    if (CardValueNotInHandBits(cardValue, handBits)) continue;

                    if (acesHigh && cardValue == (byte)Values.ACE)
                        tieBreakers = AddHighAceTieBreaker(tieBreakers);
                    else tieBreakers |= CalculateTieBreaker(cardValue, tieBreakerIndex);
                    tieBreakerIndex += 1;
                }

                return tieBreakers;
            }

            private static int AddHighAceTieBreaker(int tieBreakers)
            {
                byte highAceValue = (byte)Values.KING + 1;
                tieBreakers >>= NibbleHelpers.BITS_PER_NIBBLE;
                tieBreakers |= highAceValue << NibbleHelpers.ConvertLeftNibbleIndexToOffset(NUMBER_OF_BITS_IN_TIEBREAKER, 0);
                return tieBreakers;
            }

            internal static int CalculateTieBreaker(byte cardValue, int tieBreakerIndex)
            {
                return cardValue << NibbleHelpers.ConvertLeftNibbleIndexToOffset(
                    NUMBER_OF_BITS_IN_TIEBREAKER, tieBreakerIndex
                );
            }

            private static bool CardValueNotInHandBits(byte cardValue, ulong handBits)
            {
                ulong cardOffset = (ulong)0xF << NibbleHelpers.ConvertRightNibbleIndexToOffset(cardValue);
                ulong segment = handBits & cardOffset;
                return segment == 0;
            }

            private const int NUMBER_OF_BITS_IN_TIEBREAKER = 28;
        }

        private static class NibbleHelpers
        {
            public static int ConvertRightNibbleIndexToOffset(int pos)
            {
                return pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public static int ConvertLeftNibbleIndexToOffset(int totalBitSize, int pos)
            {
                return totalBitSize - pos * BITS_PER_NIBBLE - BITS_PER_NIBBLE;
            }

            public const int BITS_PER_NIBBLE = 4;
        }
    }
}
