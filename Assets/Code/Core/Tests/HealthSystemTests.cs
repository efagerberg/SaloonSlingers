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
                (string, int) rawSource,
                List<(string, int)> rawTargets,
                List<int> expectedTargetHealths
            )
            {
                TestSlingerAttributes source = CreateAttributes(rawSource.Item1, rawSource.Item2);
                List<TestSlingerAttributes> targets = rawTargets.Select(t => CreateAttributes(t.Item1, t.Item2)).ToList();
                HealthSystem.DoDamage(new TestHandEvaluator(), source, targets);
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
                    ("AH", 3),
                    new List<(string, int)>(),
                    new List<int>()
                },
                new object[] {
                    ("AH", 3),
                    new List<(string, int)>() { ("AH", 3) },
                    new List<int> { 3 }
                },
                new object[] {
                    ("AH", 3),
                    new List<(string, int)>() { ("AH 2C", 3) },
                    new List<int> { 3 }
                },
                new object[] {
                    ("AH 2C 3D", 3),
                    new List<(string, int)>() { ("AH", 3) },
                    new List<int> { 2 }
                },
                new object[] {
                    ("AH 2C 3D", 3),
                    new List<(string, int)>() {
                        ("AH", 3),
                        ("AH", 2)
                    },
                    new List<int> { 2, 1 }
                },
                new object[] {
                    ("AH 2C 3D", 3),
                    new List<(string, int)>() {
                        ("AH 2C 3D", 3),
                        ("AH", 2)
                    },
                    new List<int> { 3, 1 }
                },
                new object[] {
                    ("AH 2C 3D", 3),
                    new List<(string, int)>() { ("AH 2C", 0) },
                    new List<int> { 0 }
                }
            };

            private class TestSlingerAttributes : ISlingerAttributes
            {
                public IList<Card> Hand { get; set; }
                public Deck Deck { get; set; }
                public int Health { get; set; }
                public int Level { get; set; }
                public Handedness Handedness { get; set; }
            }

            private static TestSlingerAttributes CreateAttributes(string handString, int health = 3)
            {
                return new TestSlingerAttributes()
                {
                    Hand = TestHelpers.MakeHandFromString(handString).ToList(),
                    Health = health
                };
            }
        }
    }
}
