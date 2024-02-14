using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class PickupSpawner : MonoBehaviour, ISpawner<GameObject>
    {
        [SerializeField]
        private int poolSize = 10;
        [SerializeField]
        private GameObject prefab;

        private ActorPool _pool;

        public GameObject Spawn() => _pool.Get();

        private void Start()
        {
            _pool = new ActorPool(poolSize, prefab, transform);
        }
    }
}
