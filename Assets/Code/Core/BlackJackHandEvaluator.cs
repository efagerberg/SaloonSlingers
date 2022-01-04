using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public class BlackJackHandEvaluator : IHandEvaluator
    {
        private const int HIGH_ACE_VALUE = 11;
        private const int LOW_ACE_VALUE = 1;
        private const int FACE_VALUE = 10;
        private const int BUST_THRESHOLD = 21;

        public int Evaluate(IEnumerable<Card> hand)
        {
            int sum = 0;
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
                    sum += (int)card.Value;
                handLength += 1;

                if (IsBlackJack(numAces, numFaceCards, handLength)) return GetMaxHandValue();
            }

            while (sum > BUST_THRESHOLD)
            {
                if (numAces == 0) return GetMinHandValue();
                numAces -= 1;
                sum -= (HIGH_ACE_VALUE - LOW_ACE_VALUE);
            }
            return sum;
        }

        private static bool IsBlackJack(int numAces, int numFaceCards, int length)
        {
            return length == 2 && numAces == 1 && numFaceCards == 1;
        }

        private int GetMaxHandValue() => BUST_THRESHOLD + 1;
        private int GetMinHandValue() => -1;
    }
};