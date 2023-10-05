using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;


namespace SaloonSlingers.Core.Tests
{
    public class HandRotationCalculatorTests
    {
        public class TestCalculateRotations
        {
            [TestCaseSource(nameof(TestCases))]
            public void ReturnsExpectedDegrees(int n, float totalDegrees, List<float> expected)
            {
                Assert.AreEqual(
                    expected,
                    HandRotationCalculator.CalculateRotations(n, totalDegrees).ToList()
                );
            }

            private static readonly object[][] TestCases =
            {
                new object[] {
                    0,
                    30,
                    new List<float>()
                },
                new object[]
                {
                    1,
                    30,
                    new List<float> { 0 }
                },
                new object[]
                {
                    2,
                    15,
                    new List<float> { -15/2f, 15/2f }
                },
                new object[]
                {
                    3,
                    15,
                    new List<float> { -15/2f, 0, 15/2f }
                },
                new object[]
                {
                    4,
                    20,
                    new List<float> { -10, -5, 5, 10 }
                }
            };
        }
    };
}
