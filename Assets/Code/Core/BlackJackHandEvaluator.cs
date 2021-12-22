namespace SaloonSlingers.Core
{
    public class BlackJackHandEvaluator : IHandEvaluator
    {
        private const int HIGH_ACE_VALUE = 11;
        private const int LOW_ACE_VALUE = 1;
        private const int FACE_VALUE = 10;
        private const int BUST_THRESHOLD = 21;

        public float GetMaxHandValue() => BUST_THRESHOLD + 1;
        public float GetMinHandValue() => -1;

        public float Evaluate(Card[] hand)
        {
            int sum = 0;
            int numAces = 0;
            int numFaceCards = 0;
            foreach (var card in hand)
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

                if (IsBlackJack(hand, numAces, numFaceCards)) return GetMaxHandValue();
            }

            while (sum > BUST_THRESHOLD)
            {
                if (numAces == 0) return GetMinHandValue();
                numAces -= 1;
                sum -= (HIGH_ACE_VALUE - LOW_ACE_VALUE);
            }
            return sum;
        }

        private static bool IsBlackJack(Card[] hand, int numAces, int numFaceCards)
        {
            return hand.Length == 2 && numAces == 1 && numFaceCards == 1;
        }
    }
};