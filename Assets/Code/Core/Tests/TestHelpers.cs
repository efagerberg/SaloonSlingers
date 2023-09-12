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

        public static HandEvaluation EvaluateHandString(string handString, IHandEvaluator evaluator)
        {
            IEnumerable<Card> hand = MakeHandFromString(handString);
            return evaluator.Evaluate(hand);
        }

        public static string ConvertToBinaryString(int x, int padding = 0)
        {
            return Convert.ToString(x, 2).PadLeft(padding, '0');
        }

        public static Action<uint, uint> GetAssertionFromMethodString<T>(string assertionMethod)
        {
            static void areEqual(uint x, uint y) => Assert.AreEqual(x, y);
            return assertionMethod switch
            {
                "AreEqual" => areEqual,
                "Greater" => (x, y) => Assert.Greater(x, y),
                "Less" => (x, y) => Assert.Less(x, y),
                _ => areEqual,
            };
        }
    }
}