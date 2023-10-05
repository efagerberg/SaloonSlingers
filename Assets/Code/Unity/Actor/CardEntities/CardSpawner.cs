using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CardSpawner : MonoBehaviour, ISpawner<GameObject>
    {
        public GameObject Spawn() => pool.Get();

        [SerializeField]
        private int poolSize;
        [SerializeField]
        private GameObject prefab;

        private ActorPool pool;

        private void Awake()
        {
            pool = new ActorPool(poolSize, prefab, transform);
        }
    }
}
