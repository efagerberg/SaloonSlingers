using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core.Tests
{
    public class DamageApplicatorTests
    {
        public class TestDoDamage
        {
            [TestCaseSource(nameof(DoDamageTestCases))]
            public void TestDoesDamageExpectedly(
                string rawSourceHand,
                List<(string, uint)> rawTargets,
                List<uint> expectedTargetHitPoints
            )
            {
                IList<Card> sourceHand = TestHelpers.MakeHandFromString(rawSourceHand).ToList();
                IList<(Points, IList<Card>)> targets = rawTargets.Select(
                    t => (
                        new Points(t.Item2),
                        (IList<Card>)TestHelpers.MakeHandFromString(t.Item1).ToList()
                    )
                ).ToList();
                DamageApplicator.DoDamage(new TestHandEvaluator(), sourceHand, targets);
                var actualTargetHitPoints = targets.Select(x => x.Item1.Value).ToList();

                CollectionAssert.AreEqual(expectedTargetHitPoints, actualTargetHitPoints);
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
                    "AH",
                    new List<(string, uint)>(),
                    new List<uint>()
                },
                new object[] {
                    "AH",
                    new List<(string, uint)>() { ("AH", 3) },
                    new List<uint> { 3 }
                },
                new object[] {
                    "AH",
                    new List<(string, uint)>() { ("AH 2C", 3) },
                    new List<uint> { 3 }
                },
                new object[] {
                    "AH 2C 3D",
                    new List<(string, uint)>() { ("AH", 3) },
                    new List<uint> { 2 }
                },
                new object[] {
                    "AH 2C 3D",
                    new List<(string, uint)>() {
                        ("AH", 3),
                        ("AH", 2)
                    },
                    new List<uint> { 2, 1 }
                },
                new object[] {
                    "AH 2C 3D",
                    new List<(string, uint)>() {
                        ("AH 2C 3D", 3),
                        ("AH", 2)
                    },
                    new List<uint> { 3, 1 }
                },
                new object[] {
                    "AH 2C 3D",
                    new List<(string, uint)>() { ("AH 2C", 0) },
                    new List<uint> { 0 }
                }
            };
        }
    }
}
