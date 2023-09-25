using UnityEngine;
using UnityEngine.Pool;

namespace SaloonSlingers.Unity.Actor
{
    public class ActorPool : MonoBehaviour
    {
        [SerializeField]
        private int poolSize = 10;
        [SerializeField]
        public GameObject Prefab;

        private IObjectPool<GameObject> _pool;

        public GameObject Get(bool active = true)
        {
            var instance = _pool.Get();
            if (active) instance.SetActive(true);
            return instance;
        }
        public int CountActive { get; private set; } = 0;

        private void Start()
        {
            _pool = new ObjectPool<GameObject>(CreateInstance, OnGet, OnRelease, defaultCapacity: poolSize);
        }

        private GameObject CreateInstance()
        {
            GameObject instance = Instantiate(Prefab);
            instance.SetActive(false);
            instance.transform.SetParent(transform);
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return instance;
        }

        private void OnGet(GameObject instance)
        {
            var actor = instance.GetComponent<IActor>();
            actor.Death += HandleDeath;
            actor.Reset();
            CountActive++;
        }

        private void OnRelease(GameObject instance)
        {
            var actor = instance.GetComponent<IActor>();
            actor.Death -= HandleDeath;
            instance.SetActive(false);
            instance.transform.SetParent(transform);
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            CountActive--;
        }

        private void HandleDeath(object sender, System.EventArgs e)
        {
            var instance = sender as GameObject;
            _pool.Release(instance);
        }
    }
}
