using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.Unity.Tests
{
    public class ScaleByValueCalculatorTests
    {
        [TestCase(0, 1, false, 1)]
        [TestCase(1, 1, false, 2)]
        [TestCase(10, 100, false, 1.1f)]
        [TestCase(50, 100, false, 1.5f)]
        [TestCase(110, 25, false, 5.4f)]
        [TestCase(0, 1, true, 1)]
        [TestCase(40, 100, true, 1)]
        [TestCase(420, 100, true, 5)]
        public void Calculate_ReturnsExpectedScaleValue(float value, float denomination, bool quantize, float expected)
        {
            var actual = ScaleByValueCalculator.Calculate(value, denomination, quantize);
            Assert.AreEqual(expected, actual, 0.0002f);
        }
    }
}
