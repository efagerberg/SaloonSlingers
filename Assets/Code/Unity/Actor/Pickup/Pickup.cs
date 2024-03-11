﻿using System;

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
        private PositionConstraint pickupLineConstraint;
        [SerializeField]
        private ScaleByValue scaleByValue;
        [field: SerializeField]
        private uint _value;

        public void ResetActor()
        {
            Value = 0;
            rigidBody ??= GetComponent<Rigidbody>();
            pickupLineConstraint ??= GetComponent<PositionConstraint>();
            rigidBody.isKinematic = false;
            transform.localScale = Vector3.one;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            // Pickups have a nested object that moves
            rigidBody.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            pickupLineConstraint.constraintActive = true;
            pickupLineConstraint.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void TryPickup(GameObject grabber)
        {
            if (!grabber.TryGetComponent<Attributes>(out var attributes) ||
                !attributes.Registry.TryGetValue(AttributeType.Money, out var money))
                return;

            money.Increase(Value);
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }

        public void PlayerPickup()
        {
            TryPickup(LevelManager.Instance.Player);
        }


        private void OnValidate()
        {
            Value = _value;
        }
    }
}