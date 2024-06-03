using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SaloonSlingers.Unity
{
    public class CollisionDetector : MonoBehaviour
    {
        [FormerlySerializedAs("OnCollsionEntered")]
        public UnityEvent<Collision> CollisionEntered = new();
        [FormerlySerializedAs("OnCollisionExited")]
        public UnityEvent<Collision> CollisionExited = new();
        [FormerlySerializedAs("OnTriggerEntered")]
        public UnityEvent<Collider> TriggerEntered = new();
        [FormerlySerializedAs("OnTriggerExited")]
        public UnityEvent<Collider> TriggerExited = new();

        private void OnCollisionEnter(Collision other)
        {
            CollisionEntered.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            CollisionExited.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEntered.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExited.Invoke(other);
        }
    }
}
