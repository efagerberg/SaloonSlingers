using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CardSpawner : MonoBehaviour
    {
        public GameObject Spawn() => pool.Get();

        private ActorPool pool;

        private void Awake()
        {
            if (pool == null) pool = GetComponent<ActorPool>();
        }
    }
}
