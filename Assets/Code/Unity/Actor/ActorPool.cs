using UnityEngine;
using UnityEngine.Pool;

namespace SaloonSlingers.Unity.Actor
{
    public class ActorPool
    {
        private readonly GameObject prefab;
        private readonly Transform root;

        private readonly IObjectPool<GameObject> _pool;

        public ActorPool(int poolSize, GameObject prefab, Transform root)
        {
            this.root = root;
            this.prefab = prefab;
            _pool = new ObjectPool<GameObject>(CreateInstance, OnGet, OnRelease, defaultCapacity: poolSize);
            SpawnedActorCount = 0;
        }

        public GameObject Get(bool active = true)
        {
            var instance = _pool.Get();
            if (active) instance.SetActive(true);
            return instance;
        }

        public int SpawnedActorCount { get; private set; } = 0;

        private GameObject CreateInstance()
        {
            GameObject instance = Object.Instantiate(prefab);
            instance.SetActive(false);
            instance.transform.SetParent(root);
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return instance;
        }

        private void OnGet(GameObject instance)
        {
            var actor = instance.GetComponent<Actor>();
            actor.OnKilled.AddListener(OnDeath);
            SpawnedActorCount++;
        }

        private void OnRelease(GameObject instance)
        {
            var actor = instance.GetComponent<Actor>();
            actor.OnKilled.RemoveListener(OnDeath);
            instance.SetActive(false);
            actor.ResetActor();
            instance.transform.SetParent(root);
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            SpawnedActorCount--;
        }

        private void OnDeath(Actor sender)
        {
            _pool.Release(sender.gameObject);
        }
    }
}
