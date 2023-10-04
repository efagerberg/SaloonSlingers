using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class TemporaryHitPoints : MonoBehaviour
    {
        public Points Points { get; set; }

        private void Awake()
        {
            Points = new(0, uint.MaxValue);
        }

        public void Reset()
        {
            Points.Value = 0;
        }
    }
}
