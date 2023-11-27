using System;

namespace SaloonSlingers.Core.HomingStrengthCalculator
{
    public struct Config
    {
        public float BaseStrength;
        public float SigmoidSlope;
        public float BaseEffectivenessThreshold;
        public float PercentOfAverage;
        public int NMaxes;
    }

    /// <summary>
    /// Calculates the homing strength of a card throw based off of the angular velocity in the y axis, and previous throw data.
    /// </summary>
    public class Calculator
    {
        private readonly Config config;
        private readonly float[] maxes;
        private int currentIndex = 0;
        private float currentEffectivenessThreshold;

        public Calculator(Config config)
        {
            this.config = config;
            maxes = new float[config.NMaxes];
            currentEffectivenessThreshold = config.BaseEffectivenessThreshold;
        }

        public float Calculate(float yAngularVelocity)
        {
            float absYAngVel = Math.Abs(yAngularVelocity);
            float scaled = (absYAngVel - currentEffectivenessThreshold) / config.SigmoidSlope;
            float strength = config.BaseStrength * Sigmoid(config.SigmoidSlope * scaled);

            maxes[currentIndex] = Math.Max(maxes[currentIndex], absYAngVel);
            var newThreshold = CalculateEffectivenessThreshold(maxes, config.PercentOfAverage);
            if (newThreshold != null) currentEffectivenessThreshold = newThreshold.Value;

            return strength;
        }

        private static float? CalculateEffectivenessThreshold(float[] maxes, float percentOfAverage)
        {
            float sum = 0;
            int n = 0;

            foreach (var max in maxes)
            {
                if (max == default) continue;

                sum += max;
                n++;
            }

            if (n > 0) return (sum / n) * percentOfAverage;
            else return null;
        }

        public void StartNewThrow()
        {
            currentIndex = (currentIndex + 1) % config.NMaxes;
        }

        private static float Sigmoid(float x)
        {
            return (float)(1.0f / (1.0f + Math.Exp(-x)));
        }
    }
}
