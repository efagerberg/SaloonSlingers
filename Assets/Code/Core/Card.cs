using System;


namespace GambitSimulator.Core
{
    public enum Values { ACE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING }
    public enum Suits { CLUBS, DIAMONDS, HEARTS, SPADES }

    [Serializable]
    public struct Card
    {
        private const int FACE_VALUE_THRESHOLD = 10;
        public Suits Suit;
        public Values Value;


        public Card(Suits inSuit = Suits.CLUBS, Values inValue = Values.ACE)
        {
            Suit = inSuit;
            Value = inValue;
        }

        public Card(Card inCard)
        {
            Suit = inCard.Suit;
            Value = inCard.Value;
        }

        public bool IsFaceCard()
        {
            return (int)Value >= FACE_VALUE_THRESHOLD;
        }

        public override string ToString()
        {
            var value_as_int = (int)Value;
            var template = "{0}_of_{1}";
            if (value_as_int > 0 && value_as_int < FACE_VALUE_THRESHOLD)
            {
                return string.Format(template, (value_as_int + 1), Suit.ToString().ToLower());
            }
            // Use the second version of the face card
            if (IsFaceCard())
                return string.Format(template, Value.ToString().ToLower(), Suit.ToString().ToLower()) + "2";
            return string.Format(template, Value.ToString().ToLower(), Suit.ToString().ToLower());
        }

        public override int GetHashCode()
        {
            return (int)Suit ^ (int)Value;
        }
    }
}
