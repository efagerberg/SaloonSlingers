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
            set { _points = value; }
        }
        private Points _points;

        [SerializeField]
        private uint startingPoints = 5;

        public void Reset()
        {
            _points.Value = startingPoints;
        }
    }
}