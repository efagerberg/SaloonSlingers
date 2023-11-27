using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core.HomingStrengthCalculator;

namespace SaloonSlingers.Core.Tests
{
    public class HomingStrengthCalculatorTests
    {
        [Test]
        public void HigherStrengthLimitedByBaseStrength_AsYVelocityIncreases()
        {
            var config = new Config()
            {
                BaseEffectivenessThreshold = 1,
                Limit = 3,
                SigmoidSlope = 1,
                PercentOfAverage = 0.90f,
                NMaxes = 2
            };
            var calculator = new Calculator(config);
            var yVelocities = new float[] { 1f, 2f, 10f, 100f };
            var strengths = new float[yVelocities.Length];
            for (int i = 0; i < yVelocities.Length; i++)
                strengths[i] = calculator.Calculate(yVelocities[i]);

            var isIncreasing = strengths.Zip(strengths.Skip(1), (a, b) => a < b).All(x => x);
            Assert.That(isIncreasing);
            var isNormalized = strengths.All(x => x <= config.Limit);
            Assert.That(isNormalized);
        }

        [Test]
        public void ExtremeThrows_GenerateStrengthCloseToExtremes()
        {
            var config = new Config()
            {
                BaseEffectivenessThreshold = 1,
                Limit = 1,
                SigmoidSlope = 1,
                PercentOfAverage = 0.90f,
                NMaxes = 2
            };
            var calculator = new Calculator(config);
            var strongThrows = new float[] { 120f, 119f, 120f, 118f }.Select(calculator.Calculate).ToArray();
            calculator.StartNewThrow();
            var weakThrows = new float[] { 20f, 18f, 18f, 17f }.Select(calculator.Calculate).ToArray();
            var tolerance = 0.001f;

            AssertAllElementsCloseToValue(weakThrows, 0, tolerance);
            AssertAllElementsCloseToValue(strongThrows, config.Limit, tolerance);
        }

        [Test]
        public void AfterAStrongAndWeakThrow_ThrowingWithPercentOfAverageSpin_GeneratesMidPointStrength()
        {
            var config = new Config()
            {
                BaseEffectivenessThreshold = 1,
                Limit = 1,
                SigmoidSlope = 1,
                PercentOfAverage = 0.90f,
                NMaxes = 2
            };
            var calculator = new Calculator(config);
            new float[] { 120f, 119f, 120f, 118f }.Select(calculator.Calculate).ToArray();
            calculator.StartNewThrow();
            new float[] { 20f, 18f, 18f, 17f }.Select(calculator.Calculate).ToArray();
            var expectedThresolhold = config.PercentOfAverage * ((120 + 20) / 2f);
            calculator.StartNewThrow();
            var strength = calculator.Calculate(expectedThresolhold);

            Assert.That(strength, Is.EqualTo(config.Limit / 2f));
        }

        private static void AssertAllElementsCloseToValue<IComparable>(IEnumerable<IComparable> xs, IComparable value, float tolerance)
        {
            foreach (var x in xs)
            {
                Assert.That(x, Is.EqualTo(value).Within(tolerance));
            }
        }
    }
}
