using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public enum Suits { CLUBS, DIAMONDS, HEARTS, SPADES }
    public enum Values
    {
        ACE = 1, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING
    }

    [Serializable]
    public struct Card
    {
        private static Dictionary<char, Suits> CharToSuit = Enum.GetValues(typeof(Suits))
                                                                 .Cast<Suits>()
                                                                 .ToDictionary(x => x.ToString()[0], y => y);
        private static Dictionary<char, Values> CharToValue = Enum.GetValues(typeof(Values))
                                                                  .Cast<Values>()
                                                                  .ToDictionary(x => (int)x >= FACE_VALUE_THRESHOLD || x == Values.ACE ? x.ToString()[0] : ((int)x).ToString()[0], y => y);
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

        public Card(String cardString)
        {
            Value = CharToValue[cardString[0]];
            Suit = CharToSuit[cardString[1]];
        }

        public bool IsFaceCard()
        {
            int valAsInt = (int)Value;
            return valAsInt > FACE_VALUE_THRESHOLD;
        }

        public override string ToString()
        {
            var value_as_int = (int)Value;
            var template = "{0}_of_{1}";

            if (IsFaceCard() || Value == Values.ACE)
                return string.Format(template, Value.ToString().ToLower(), Suit.ToString().ToLower());

            return string.Format(template, (value_as_int), Suit.ToString().ToLower());
        }

        public override int GetHashCode()
        {
            return (int)Suit ^ (int)Value;
        }
    }
}
