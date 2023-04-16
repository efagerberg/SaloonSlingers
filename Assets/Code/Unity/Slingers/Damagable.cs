using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class Damagable : MonoBehaviour
    {
        public Points Health { get; private set; }

        [SerializeField]
        private uint startingHealthPoints = 5;

        private void Start()
        {
            Health = new Points(startingHealthPoints);
        }
    }
}
