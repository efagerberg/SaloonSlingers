using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class TemporaryHitPoints : MonoBehaviour
    {
        public Points Points { get; set; }

        public void Reset()
        {
            Points.Value = 0;
        }
    }
}
