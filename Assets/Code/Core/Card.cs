using System;


namespace GambitSimulator.Core
{
    public enum Values { ACE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING }
    public enum Suits { CLUBS, DIAMONDS, HEARTS, SPADES }

    [Serializable]
    public struct Card
    {
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

        public override bool Equals(object other)
        {
            if (other == null) return false;
            var card = (Card)other;
            return (Suit == card.Suit) &&
                   (Value == card.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsFaceCard()
        {
            return (int)Value >= 10;
        }

        public override string ToString()
        {
            var value_as_int = (int)Value;
            var template = "{0}_of_{1}";
            if (value_as_int > 0 && value_as_int < 10)
            {
                return string.Format(template, (value_as_int + 1), Suit.ToString().ToLower());
            }
            // Use the second version of the face card
            if (IsFaceCard())
                return string.Format(template, Value.ToString().ToLower(), Suit.ToString().ToLower()) + "2";
            return string.Format(template, Value.ToString().ToLower(), Suit.ToString().ToLower());
        }
    }
}
