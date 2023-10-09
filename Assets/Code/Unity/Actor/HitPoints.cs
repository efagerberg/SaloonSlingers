using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HitPoints : MonoBehaviour
    {
        public Core.HitPoints Points
        {
            get
            {
                _points ??= new Core.HitPoints(startingPoints);
                return _points;
            }
            private set { _points = value; }
        }
        private Core.HitPoints _points;

        [SerializeField]
        private uint startingPoints = 5;
    }
}
