using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HitPoints : MonoBehaviour
    {
        public Core.Points Points
        {
            get
            {
                _points ??= new Core.Points(startingPoints);
                return _points;
            }
            private set { _points = value; }
        }
        private Core.Points _points;

        [SerializeField]
        private uint startingPoints = 5;
    }
}
