using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// Calculates the homing strength of a card throw based off of the angular velocity in the y axis, and previous throw data.
    /// </summary>
    public class HomingStrengthCalculator : MonoBehaviour
    {
        [SerializeField]
        private float baseStrength = 4f;
        [SerializeField]
        private float sigmoidSlope = 1f;
        [SerializeField]
        private float effectivenessThreshold = 1f;
        [SerializeField]
        [Range(0, 1)]
        private float percentOfAverage = 0.85f;

        [SerializeField]
        private int nMaxesToTrack = 5;
        
        private float[] maxes;
        private int currentIndex = 0;

    public float Calculate(float yAngularVelocity)
    {
        float absYAngVel = Mathf.Abs(yAngularVelocity);
        float scaled = (absYAngVel - effectivenessThreshold) / sigmoidSlope;
        float strength = baseStrength * Sigmoid(sigmoidSlope * scaled);

        maxes[currentIndex] = Mathf.Max(maxes[currentIndex], absYAngVel);
        var newThreshold = CalculateEffectivenessThreshold(maxes, percentOfAverage);
        if (newThreshold != null) effectivenessThreshold = newThreshold.Value;
            
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
            currentIndex = (currentIndex + 1) % nMaxesToTrack;
        }

        private static float Sigmoid(float x)
        {
            return 1.0f / (1.0f + Mathf.Exp(-x));
        }

        private void Awake()
        {
            maxes = new float[nMaxesToTrack];
        }
    }
}
