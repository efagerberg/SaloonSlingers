using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public static class TestHelpers
    {
        public static IEnumerable<Card> MakeHandFromString(string handString)
        {
            if (handString.Length == 0) yield break;
            foreach (string cardString in handString.Split(' '))
                yield return new Card(cardString);
        }

        public static string ConvertToBinaryString(int x, int padding = 0)
        {
            return Convert.ToString(x, 2).PadLeft(padding, '0');
        }

        public static Action<uint, uint> GetAssertionFromMethodString(string assertionMethod)
        {
            static void areEqual(uint x, uint y) => Assert.AreEqual(x, y);
            return assertionMethod switch
            {
                "AreEqual" => areEqual,
                "Greater" => Assert.Greater,
                "Less" => Assert.Less,
                _ => areEqual,
            };
        }
    }
}