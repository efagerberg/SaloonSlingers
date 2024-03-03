using System;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Animations;

namespace SaloonSlingers.Unity
{
    [ExecuteAlways]
    public class Pickup : MonoBehaviour, IActor
    {
        public uint Value;
        public event EventHandler Killed;

        [SerializeField]
        private Rigidbody rigidBody;
        [SerializeField]
        private PositionConstraint pickupLineConstraint;

        private ScaleByValue scaleByValue;

        private void Awake()
        {
            scaleByValue = GetComponent<ScaleByValue>();
            scaleByValue.Scale(Value);
        }

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
    }
}
