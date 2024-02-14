using System;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class Pickup : MonoBehaviour, IActor
    {
        public uint Value;
        public event EventHandler Death;

        public void ResetActor()
        {
            Value = 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Player") &&
                collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
                return;

            if (collision.gameObject.TryGetComponent<Attributes>(out var attributes) &&
                attributes.Registry.TryGetValue(AttributeType.Money, out var money))
                money.Increase(Value);
            Death?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
