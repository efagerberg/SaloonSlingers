namespace SaloonSlingers.Core.HomingStrengthCalculator
{
    public struct Config
    {
        public float Limit;
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
            float absYAngVel = System.Math.Abs(yAngularVelocity);
            float scaled = (absYAngVel - currentEffectivenessThreshold) / config.SigmoidSlope;
            float strength = config.Limit * Math.Sigmoid(config.SigmoidSlope * scaled);

            maxes[currentIndex] = System.Math.Max(maxes[currentIndex], absYAngVel);
            var newThreshold = CalculateEffectivenessThreshold(maxes, config.PercentOfAverage);
            if (newThreshold >= 0)
                currentEffectivenessThreshold = CalculateEffectivenessThreshold(maxes, config.PercentOfAverage);

            return strength;
        }

        private static float CalculateEffectivenessThreshold(float[] maxes, float percentOfAverage)
        {
            float sum = 0;
            int n = 0;

            foreach (var max in maxes)
            {
                if (max == default) continue;

                sum += max;
                n++;
            }
            if (n == 0) return -1;

            return (sum / n) * percentOfAverage;
        }

        public void StartNewThrow()
        {
            currentIndex = (currentIndex + 1) % config.NMaxes;
        }
    }
}
