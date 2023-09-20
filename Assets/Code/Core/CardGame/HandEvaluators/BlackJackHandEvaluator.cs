using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public class BlackJackHandEvaluator : IHandEvaluator
    {
        private const int HIGH_ACE_VALUE = 11;
        private const int LOW_ACE_VALUE = 1;
        private const int FACE_VALUE = 10;
        private const int BUST_THRESHOLD = 21;
        // A 21 without black jack should be less than a 21 with blackjack
        private const int MAX_HAND_VALUE = 21 + 1;
        private const int MIN_HAND_VALUE = 0;

        // Note we are offsetting the actual hand value by 2
        // This is because we can't have negative scores.
        // 0 means you busted
        // 1 means no hand
        // >2 everything else
        private const int OFFSET = 2;

        public HandEvaluation Evaluate(IEnumerable<Card> hand)
        {
            uint sum = OFFSET;
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
                    sum += (uint)card.Value;
                handLength += 1;
            }
            if (sum == OFFSET) return new HandEvaluation(HandNames.NONE, sum);
            if (IsBlackJack(numAces, numFaceCards, handLength)) return new HandEvaluation(HandNames.BLACK_JACK, MAX_HAND_VALUE);

            while (sum > BUST_THRESHOLD + OFFSET)
            {
                if (numAces == 0) return new HandEvaluation(HandNames.BUST, MIN_HAND_VALUE);
                numAces -= 1;
                sum -= (HIGH_ACE_VALUE - LOW_ACE_VALUE);
            }
            int startingEnumValue = (int)HandNames.ONE - 1;
            return new HandEvaluation((HandNames)(startingEnumValue + sum - OFFSET), sum);
        }

        private static bool IsBlackJack(int numAces, int numFaceCards, int length)
        {
            return length == 2 && numAces == 1 && numFaceCards == 1;
        }
    }
};