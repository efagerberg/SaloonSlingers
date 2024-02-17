using System;
using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class DropOnDeath : MonoBehaviour
    {
        public Transform DropPositionReference;

        private Attributes attributes;
        private IActor actor;

        private void OnEnable()
        {
            actor = GetComponent<IActor>();
            actor.Death += OnDeath;
        }

        private void Start()
        {
            if (DropPositionReference == null) DropPositionReference = transform;
            attributes = GetComponent<Attributes>();
        }

        private void OnDisable()
        {
            actor.Death -= OnDeath;
        }

        private void OnDeath(object sender, EventArgs e)
        {
            PickupDropper.Drop(attributes.Registry,
                               LevelManager.Instance.PickupSpawner,
                               gameObject.layer,
                               DropPositionReference.position);
        }
    }

    public static class PickupDropper
    {
        public static void Drop(IDictionary<AttributeType, Core.Attribute> registry,
                                ISpawner<GameObject> pickupSpawner,
                                int layerToAssign,
                                Vector3 dropPosition)
        {
            if (!registry.TryGetValue(AttributeType.Pot, out var pot) ||
                !registry.TryGetValue(AttributeType.Money, out var money)) return;

            var spawnedPickup = pickupSpawner.Spawn();
            spawnedPickup.layer = layerToAssign;
            spawnedPickup.transform.position = dropPosition;
            float scaleFactor = 1 + (float)pot.Value / money.InitialValue;
            spawnedPickup.transform.localScale = Vector3.one * scaleFactor;
            var pickup = spawnedPickup.GetComponent<Pickup>();
            pickup.Value = pot;
        }
    }
}
