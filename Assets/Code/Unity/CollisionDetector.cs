using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class CollisionDetector : MonoBehaviour
    {
        public UnityEvent<Collision> OnCollsionEntered = new();
        public UnityEvent<Collision> OnCollisionExited = new();
        public UnityEvent<Collider> OnTriggerEntered = new();
        public UnityEvent<Collider> OnTriggerExited = new();

        private void OnCollisionEnter(Collision other)
        {
            OnCollsionEntered.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnCollisionExited.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEntered.Invoke(other);
        }
        private void OnTriggerExit(Collider other)
        {
            OnTriggerExited.Invoke(other);
        }
    }
}