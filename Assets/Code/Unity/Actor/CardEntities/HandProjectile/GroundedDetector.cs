using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class GroundedDetector : MonoBehaviour
    {
        public bool IsGrounded { get; private set; } = false;

        public UnityEvent<Collision> OnGroundedEnter;
        public UnityEvent<Collision> OnGroundedExit;

        [SerializeField]
        private Rigidbody rigidBody;
        [SerializeField]
        private LayerMask groundLayerMask;
        [SerializeField]
        private float maxSpeed = 0.8f;

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsGrounded && collision.gameObject.IsInLayerMask(groundLayerMask) && rigidBody.velocity.y < maxSpeed)
            {
                OnGroundedEnter.Invoke(collision);
                IsGrounded = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (IsGrounded && collision.gameObject.IsInLayerMask(groundLayerMask))
            {
                OnGroundedExit.Invoke(collision);
                IsGrounded = false;
            }
        }
    }
}
