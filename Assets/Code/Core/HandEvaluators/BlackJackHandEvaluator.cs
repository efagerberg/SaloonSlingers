using System.Collections.Generic;

namespace SaloonSlingers.Core.HandEvaluators
{
    public class BlackJackHandEvaluator : IHandEvaluator
    {
        // Note we are offsetting values by 2, since we can't have negative scores.
        // 0 means you busted
        // 1 means no hand
        // >2 everything else
        private const int HIGH_ACE_VALUE = 11 + 2;
        private const int LOW_ACE_VALUE = 1 + 2;
        private const int FACE_VALUE = 10 + 2;
        private const int BUST_THRESHOLD = 21 + 2;

        public HandType Evaluate(IEnumerable<Card> hand)
        {
            uint sum = 1;
            int numAces = 0;
            int numFaceCards = 0;
            int handLength = 0;
            foreach (Card card in hand)
            {
                if (card.Value == Values.ACE)
                {
                    sum += HIGH_ACE_VALUE;
                    numAces += 1;
                }
                else if (card.IsFaceCard())
                {
                    sum += FACE_VALUE;
                    numFaceCards += 1;
                }
                else
                    sum += (uint)card.Value + 2;
                handLength += 1;

                if (IsBlackJack(numAces, numFaceCards, handLength)) return new HandType(HandNames.BLACK_JACK, GetMaxHandValue());
            }

            while (sum > BUST_THRESHOLD)
            {
                if (numAces == 0) return new HandType(HandNames.BUST, GetMinHandValue());
                numAces -= 1;
                sum -= (HIGH_ACE_VALUE - LOW_ACE_VALUE);
            }
            return new HandType(HandNames.EMPTY, sum);
        }

        private static bool IsBlackJack(int numAces, int numFaceCards, int length)
        {
            return length == 2 && numAces == 1 && numFaceCards == 1;
        }

        private uint GetMaxHandValue() => BUST_THRESHOLD + 1;
        private uint GetMinHandValue() => 0;
    }
};