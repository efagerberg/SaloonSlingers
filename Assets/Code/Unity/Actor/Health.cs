using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Health : MonoBehaviour
    {
        public Points Points
        {
            get
            {
                _points ??= new Points(startingHealthPoints);
                return _points;
            }
            set { _points = value; }
        }
        private Points _points;

        [SerializeField]
        private uint startingHealthPoints = 5;

        public void Reset()
        {
            _points.Value = startingHealthPoints;
        }
    }
}
