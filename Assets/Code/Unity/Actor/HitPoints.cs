using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HitPoints : MonoBehaviour
    {
        [SerializeField]
        private uint startingPoints = 5;
        [SerializeField]
        private uint maxPoints;

        public Points Points
        {
            get
            {
                if (maxPoints == 0) maxPoints = startingPoints;
                _points ??= new Points(startingPoints, maxPoints);
                return _points;
            }
            private set { _points = value; }
        }

        public static implicit operator uint(HitPoints hp)
        {
            return hp.Points;
        }

        private Points _points;
    }
}
