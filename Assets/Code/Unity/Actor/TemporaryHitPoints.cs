using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class TemporaryHitPoints : MonoBehaviour
    {
        public Points Points
        {
            get
            {
                _points ??= new Points(0, uint.MaxValue);
                return _points;
            }
            private set { _points = value; }
        }

        private Points _points;
    }
}
