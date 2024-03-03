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
        public Suits Suit { get; private set; }
        public Values Value { get; private set; }

        private static readonly Dictionary<char, Values> CharToValue = Enum.GetValues(typeof(Values))
                                                          .Cast<Values>()
                                                          .ToDictionary(x => (int)x >= FACE_VALUE_THRESHOLD || x == Values.ACE ? x.ToString()[0] : ((int)x).ToString()[0], y => y);
        private static readonly Dictionary<char, Suits> CharToSuit = Enum.GetValues(typeof(Suits))
                                                                 .Cast<Suits>()
                                                                 .ToDictionary(x => x.ToString()[0], y => y);
        private const int FACE_VALUE_THRESHOLD = 10;

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
            int value_as_int = (int)Value;
            string template = "{0}_of_{1}";

            if (IsFaceCard() || Value == Values.ACE)
                return string.Format(template, Value.ToString().ToLower(), Suit.ToString().ToLower());

            return string.Format(template, (value_as_int), Suit.ToString().ToLower());
        }

        public string ToUnicode()
        {
            int value_as_int = (int)Value;
            char suitUnicode = GetSuitUnicode(Suit);
            if (IsFaceCard() || Value == Values.ACE)
                return $"{Value.ToString().ToUpper()[0]}{suitUnicode}";

            return $"{value_as_int}{suitUnicode}";
        }

        private char GetSuitUnicode(Suits suit)
        {
            Dictionary<Suits, char> suitToChar = new() {
                { Suits.DIAMONDS, '\u2666' },
                { Suits.SPADES, '\u2660' },
                { Suits.HEARTS, '\u2665' },
                { Suits.CLUBS, '\u2663' }
            };
            return suitToChar[suit];
        }

        public override int GetHashCode() => Encode(this);

        /// <summary>
        /// Encodes a Card object into a byte
        /// Scheme: vvvvcdhs
        ///     v = value of card (ace=1,two=2,three=3,four=4,five=5,...,king=13)
        ///     cdhs = suit of card (bit turned on based on suit of card)
        /// </summary>
        public static byte Encode(Card card)
        {
            return (byte)(GetValueMask(card) | GetSuitMask(card));
        }

        public static Card Decode(byte encodedCard)
        {
            Values value = (Values)(GetValueMask(encodedCard));
            Suits suit = (Suits)(-System.Math.Log(GetSuitMask(encodedCard), 2) + 3);
            return new Card(value, suit);
        }

        public static byte GetSuitMask(byte encodedCard) => (byte)(encodedCard & 0x0F);
        public static byte GetSuitMask(Card card) => (byte)System.Math.Pow(2, 3 - ((int)card.Suit));
        public static byte GetValueMask(byte encodedCard) => (byte)(encodedCard >> 4);
        public static byte GetValueMask(Card card) => (byte)((byte)card.Value << 4);
    }
}
