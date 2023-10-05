using UnityEngine;

namespace SaloonSlingers.Unity
{
    [RequireComponent(typeof(Rigidbody))]
    public class Homable : MonoBehaviour
    {
        public Transform Target;
        public float Strength;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Target == null) return;

            Vector3 currentVelocity = rb.velocity;
            Vector3 directionToTarget = (Target.position - transform.position).normalized;
            Vector3 velocityChange = Vector3.Scale(
                directionToTarget * currentVelocity.magnitude - currentVelocity,
                Vector3.one * Strength
            );
            rb.AddForce(velocityChange * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
