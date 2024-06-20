using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class Projectile : MonoBehaviour
    {
        public UnityEvent<Projectile> Thrown = new();
        
        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;

        public void Throw()
        {
            rigidBody.isKinematic = false;
            Thrown.Invoke(this);
        }

        public void Throw(Vector3 offset)
        {
            Throw();
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
        }

        public void Throw(Vector3 offset, Vector3 torque) {
            Throw(offset);
            rigidBody.AddTorque(torque, ForceMode.VelocityChange);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
        }
    }
}
