using UnityEngine;

using CalculatorLib = SaloonSlingers.Core.HomingStrengthCalculator;

namespace SaloonSlingers.Unity.Actor
{
    public class HomingStrength : MonoBehaviour
    {
        [SerializeField]
        private float limit = 4f;
        [SerializeField]
        private float sigmoidSlope = 1f;
        [SerializeField]
        private float baseEffectivenessThreshold = 1f;
        [SerializeField]
        [Range(0, 1)]
        private float percentOfAverage = 0.85f;

        [SerializeField]
        private int nMaxes = 5;

        public CalculatorLib.Calculator Calculator { get; private set; }

        private void Awake()
        {
            CalculatorLib.Config config = new()
            {
                Limit = limit,
                BaseEffectivenessThreshold = baseEffectivenessThreshold,
                NMaxes = nMaxes,
                PercentOfAverage = percentOfAverage,
                SigmoidSlope = sigmoidSlope
            };
            Calculator = new(config);
        }
    }
}
