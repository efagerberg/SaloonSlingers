using System;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Animations;

namespace SaloonSlingers.Unity
{
    [RequireComponent(typeof(ScaleByValue))]
    public class Pickup : MonoBehaviour, IActor, IPickup
    {
        public event EventHandler Killed;

        [field: SerializeField]
        public uint Value { get; set; }

        [SerializeField]
        private Rigidbody rigidBody;
        [SerializeField]
        private PositionConstraint pickupLineConstraint;
        [SerializeField]
        private ScaleByValue scaleByValue;

        public void ResetActor()
        {
            Value = 0;
            rigidBody.isKinematic = false;
            transform.localScale = Vector3.one;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            // Pickups have a nested object that moves
            rigidBody.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            pickupLineConstraint.constraintActive = true;
            pickupLineConstraint.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void TryPickup(GameObject target)
        {
            if (target.TryGetComponent<Attributes>(out var attributes) &&
                attributes.Registry.TryGetValue(AttributeType.Money, out var money))
            {
                money.Increase(Value);
                Killed?.Invoke(gameObject, EventArgs.Empty);
            }
        }

        public void PlayerPickup()
        {
            TryPickup(LevelManager.Instance.Player);
        }


        private void OnValidate()
        {
            if (scaleByValue == null) return;


            scaleByValue.Scale(transform, Value);
        }
    }
}
