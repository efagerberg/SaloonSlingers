using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class GroundedDetector : MonoBehaviour
    {
        public bool IsGrounded
        {
            get => isGrounded;
            private set
            {
                isGrounded = value;
                if (isGrounded) OnGroundedEnter.Invoke();
                else OnGroundedExit.Invoke();
            }
        }

        public UnityEvent OnGroundedEnter;
        public UnityEvent OnGroundedExit;

        [SerializeField]
        private LayerMask groundLayerMask;
        [SerializeField]
        private float maxSpeed = 0.8f;

        private Rigidbody rigidBody;
        private bool isGrounded = false;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            IsGrounded = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsGrounded && collision.gameObject.IsInLayerMask(groundLayerMask) && rigidBody.velocity.y < maxSpeed)
                IsGrounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (IsGrounded && collision.gameObject.IsInLayerMask(groundLayerMask))
                IsGrounded = false;
        }
    }
}
