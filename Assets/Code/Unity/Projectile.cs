using SaloonSlingers.Core;
using System;

using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SaloonSlingers.Unity
{
    public class Projectile : MonoBehaviour
    {
        public UnityEvent<Projectile> Thrown = new();
        
        [SerializeField]
        private int maxAngularVelocity = 100;

        private Rigidbody rigidBody;

        public void Throw(Vector3 offset)
        {
            Thrown.Invoke(this);
            rigidBody.AddForce(offset, ForceMode.VelocityChange);
        }

        public void Throw()
        {
            rigidBody.isKinematic = true;
            Thrown.Invoke(this);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
        }
    }
}
