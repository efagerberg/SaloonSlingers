using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HitPoints : MonoBehaviour
    {
        public Points Points
        {
            get
            {
                _points ??= new Points(startingPoints);
                return _points;
            }
            private set { _points = value; }
        }
        private Points _points;

        [SerializeField]
        private uint startingPoints = 5;
    }
}
