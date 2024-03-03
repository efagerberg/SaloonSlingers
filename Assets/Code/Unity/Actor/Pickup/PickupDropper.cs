using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public static class PickupDropper
    {
        public static void Drop(IDictionary<AttributeType, Attribute> registry,
                                ISpawner<GameObject> pickupSpawner,
                                int layerToAssign,
                                Vector3 dropPosition)
        {
            if (!registry.TryGetValue(AttributeType.Pot, out var pot)) return;

            var spawnedPickup = pickupSpawner.Spawn();
            spawnedPickup.transform.position = dropPosition;
            spawnedPickup.layer = layerToAssign;
            var pickup = spawnedPickup.GetComponent<Pickup>();
            pickup.Value = pot;
        }
    }
}
