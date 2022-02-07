using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core.HandEvaluators;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Core.Tests
{
    public class HealthSystemTests
    {
        public class TestDoDamage
        {
            [TestCaseSource(nameof(DoDamageTestCases))]
            public void TestDoesDamageExpectedly(
                ISlingerAttributes source,
                List<ISlingerAttributes>
                targets, List<int> expectedTargetHealths
            )
            {
                HealthSystem.DoDamage(source, targets);
                var actualTargetHealths = targets.Select(x => x.Health).ToList();

                Assert.AreEqual(expectedTargetHealths, actualTargetHealths);
            }

            private class TestHandEvaluator : IHandEvaluator
            {
                public HandType Evaluate(IEnumerable<Card> hand)
                {
                    return new HandType(HandNames.FOUR_OF_A_KIND, (uint)hand.Count());
                }
            }

            private static readonly object[][] DoDamageTestCases = {
                new object[] {
                    CreateAttributes("AH", 3),
                    new List<ISlingerAttributes>(),
                    new List<int>()
                },
                new object[] {
                    CreateAttributes("AH", 3),
                    new List<ISlingerAttributes>() { CreateAttributes("AH", 3) },
                    new List<int> { 3 }
                },
                new object[] {
                    CreateAttributes("AH", 3),
                    new List<ISlingerAttributes>() { CreateAttributes("AH 2C", 3) },
                    new List<int> { 3 }
                },
                new object[] {
                    CreateAttributes("AH 2C 3D", 3),
                    new List<ISlingerAttributes>() { CreateAttributes("AH", 3) },
                    new List<int> { 2 }
                },
                new object[] {
                    CreateAttributes("AH 2C 3D", 3),
                    new List<ISlingerAttributes>() {
                        CreateAttributes("AH", 3),
                        CreateAttributes("AH", 2)
                    },
                    new List<int> { 2, 1 }
                },
                new object[] {
                    CreateAttributes("AH 2C 3D", 3),
                    new List<ISlingerAttributes>() {
                        CreateAttributes("AH 2C 3D", 3),
                        CreateAttributes("AH", 2)
                    },
                    new List<int> { 3, 1 }
                },
                new object[] {
                    CreateAttributes("AH 2C 3D", 3),
                    new List<ISlingerAttributes>() { CreateAttributes("AH 2C", 0) },
                    new List<int> { 0 }
                }
            };

            private class TestSlingerAttributes : ISlingerAttributes
            {
                public Hand Hand { get; set; }
                public int MaxHandSize { get; set; }
                public int Health { get; set; }
            }

            private static TestSlingerAttributes CreateAttributes(string handString, int health = 3)
            {
                return new TestSlingerAttributes
                {
                    Hand = new Hand(
                        new TestHandEvaluator(),
                        TestHelpers.MakeHandFromString(handString)
                    ),
                    Health = health
                };
            }
        }
    }
}
