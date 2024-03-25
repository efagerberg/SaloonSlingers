using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    [RequireComponent(typeof(ScaleByValue))]
    public class Pickup : Actor.Actor, IPickup
    {
        public uint Value
        {
            get => _value;
            set
            {
                _value = value;
                scaleByValue ??= GetComponent<ScaleByValue>();
                scaleByValue.Scale(transform, value);
            }
        }

        [SerializeField]
        private Rigidbody rigidBody;
        [SerializeField]
        private ScaleByValue scaleByValue;
        [field: SerializeField]
        private uint _value;

        public override void ResetActor()
        {
            Value = 0;
            rigidBody ??= GetComponent<Rigidbody>();
            rigidBody.isKinematic = false;
            transform.localScale = Vector3.one;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            // Pickups have a nested object that moves
            rigidBody.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.layer = LayerMask.NameToLayer("Pickup");
        }

        public void TryPickup(GameObject grabber)
        {
            if (!grabber.TryGetComponent<Attributes>(out var attributes) ||
                !attributes.Registry.TryGetValue(AttributeType.Money, out var money))
                return;

            money.Increase(Value);
            OnKilled?.Invoke(gameObject);
        }

        public void PlayerPickup()
        {
            TryPickup(LevelManager.Instance.Player);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Value = _value;
        }
# endif
    }
}
