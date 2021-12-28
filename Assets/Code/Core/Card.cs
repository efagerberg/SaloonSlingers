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
        private static Dictionary<char, Values> CharToValue = Enum.GetValues(typeof(Values))
                                                          .Cast<Values>()
                                                          .ToDictionary(x => (int)x >= FACE_VALUE_THRESHOLD || x == Values.ACE ? x.ToString()[0] : ((int)x).ToString()[0], y => y);
        private static Dictionary<char, Suits> CharToSuit = Enum.GetValues(typeof(Suits))
                                                                 .Cast<Suits>()
                                                                 .ToDictionary(x => x.ToString()[0], y => y);
        private const int FACE_VALUE_THRESHOLD = 10;
        public Suits Suit;
        public Values Value;

        public Card(Values inValue = Values.ACE, Suits inSuit = Suits.CLUBS)
        {
            Value = inValue;
            Suit = inSuit;
        }

        public Card(Card inCard)
        {
            Value = inCard.Value;
            Suit = inCard.Suit;
        }

        public Card(string cardString)
        {
            Value = CharToValue[cardString[0]];
            Suit = CharToSuit[cardString[1]];
        }

        public bool IsFaceCard()
        {
            int valAsInt = (int)Value;
            return valAsInt > FACE_VALUE_THRESHOLD && Value != Values.ACE;
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
            return Encode(this);
        }

        /// <summary>
        /// Encodes a Card object into a byte
        /// Scheme: vvvvcdhs
        ///     v = value of card (ace=1,deuce=2,trey=3,four=4,five=5,...,king=13)
        ///     cdhs = suit of card (bit turned on based on suit of card)
        /// </summary>
        public static byte Encode(Card card)
        {
            byte suitByte = (byte)Math.Pow(2, 3 - ((int)card.Suit));
            return (byte)((byte)card.Value << 4 | suitByte);
        }

        public static Card Decode(byte encodedCard)
        {
            Values value = (Values)(encodedCard >> 4);
            Suits suit = (Suits)(-Math.Log(encodedCard & 0b_0000_1111, 2) + 3);
            return new Card(value, suit);
        }
    }
}
