using System;
using System.Linq;

using NUnit.Framework;


namespace SaloonSlingers.Core.Tests
{
    public class TestCalculateFade
    {
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        public void Test_TransitionsFromStartValueToEnd(float start, float end)
        {
            static float getDeltaTime() => 0.1f;
            var config = new FadeConfig()
            {
                Duration = 1,
                Start = start,
                End = end,
                GetDeltaTime = getDeltaTime,
            };
            var steps = AnimationCalculator.CalculateFade(config).ToList();
            bool shouldAscend = config.Start <= config.End;

            for (int i = 0; i < steps.Count - 1; i++)
            {
                if (shouldAscend) Assert.GreaterOrEqual(steps[i + 1], steps[i]);
                else Assert.LessOrEqual(steps[i + 1], steps[i]);
            }
            Assert.That(steps[^1], Is.EqualTo(config.End));
        }

        [TestCase(1, 0)]
        [TestCase(0, 1)]
        public void Test_Linear(float start, float end)
        {
            float getDeltaTime() => 0.1f;
            var config = new FadeConfig()
            {
                Duration = 1f,
                Start = start,
                End = end,
                GetDeltaTime = getDeltaTime,
            };
            var steps = AnimationCalculator.CalculateFade(config).ToList();

            var previousChange = Math.Abs(steps[1] - steps[0]);
            for (int i = 2; i < steps.Count - 1; i++)
            {
                var currentChange = Math.Abs(steps[i + 1] - steps[i]);
                Assert.AreEqual(currentChange, previousChange, 0.0001f);
                previousChange = currentChange;
            }
        }

        [Test]
        public void Test_TransitionsInDuration()
        {
            static float getDeltaTime() => 0.1f;
            var config = new FadeConfig()
            {
                Duration = 1,
                Start = 0,
                End = 1,
                GetDeltaTime = getDeltaTime,
            };
            var nSteps = AnimationCalculator.CalculateFade(config).Count();

            Assert.That(nSteps * 0.1, Is.EqualTo(config.Duration));
        }

        [Test]
        public void Test_TransitionsInDurationWithVariableDeltaTime()
        {
            int i = 0;
            float getDeltaTime()
            {
                var x = mockDeltas[i];
                i++;
                return x;
            }
            var config = new FadeConfig()
            {
                Duration = 0.5f,
                Start = 0,
                End = 1,
                GetDeltaTime = getDeltaTime,
            };
            var steps = AnimationCalculator.CalculateFade(config);

            Assert.That(steps.Select((_, index) => mockDeltas[index]).Sum(), Is.EqualTo(config.Duration));
        }

        private static float[] mockDeltas = new[]
        {
            0.1f,
            0.01f,
            0.09f,
            0.1f,
            0.1f,
            0.05f,
            0.05f,
        };
    }
}
