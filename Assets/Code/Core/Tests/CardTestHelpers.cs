using System.Collections.Generic;

namespace SaloonSlingers.Core.Tests
{
    public static class CardTestHelpers
    {
        public static IEnumerable<Card> MakeHandFromString(string handString)
        {
            if (handString.Length == 0) yield break;
            foreach (string cardString in handString.Split(' '))
                yield return new Card(cardString);
        }
    }
}