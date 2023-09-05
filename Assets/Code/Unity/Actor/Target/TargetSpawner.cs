using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class TargetSpawner : MonoBehaviour
    {
        [SerializeField]
        private float spawnPerSecond = 1f;
        [SerializeField]
        private int maxSpawned = 5;
        [SerializeField]
        private ActorPool pool;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            if (pool != null) pool = GetComponent<ActorPool>();

            InvokeRepeating(nameof(Spawn), 1, spawnPerSecond);
        }

        private void Spawn()
        {
            if (pool.CountActive == maxSpawned) return;

            Vector3 spawnPoint = new(
                Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
                Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
                Random.Range(_collider.bounds.min.z, _collider.bounds.max.z)
            );
            var go = pool.Get();
            go.transform.position = spawnPoint;
        }
    }
}
