using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
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
            spawnedPickup.transform.position = dropPosition;
            float scaleFactor = 1 + (float)pot.Value / money.InitialValue;
            spawnedPickup.transform.localScale = Vector3.one * scaleFactor;
            var pickup = spawnedPickup.GetComponent<Pickup>();
            pickup.Value = pot;
        }
    }
}
