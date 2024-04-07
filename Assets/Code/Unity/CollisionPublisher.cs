using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class CollisionPublisher : MonoBehaviour
    {
        public UnityEvent<Collision> OnCollided = new();
        public UnityEvent<Collider> OnTriggered = new();

        private void OnCollisionEnter(Collision other)
        {
            OnCollided.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggered.Invoke(other);
        }
    }
}