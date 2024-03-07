using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public static class PickupDropper
    {
        public static void Drop(Attribute attribute,
                                ISpawner<GameObject> pickupSpawner,
                                int layerToAssign,
                                Vector3 dropPosition)
        {
            var spawnedPickup = pickupSpawner.Spawn();
            spawnedPickup.transform.position = dropPosition;
            spawnedPickup.layer = layerToAssign;
            var pickup = spawnedPickup.GetComponent<IPickup>();
            pickup.Value = attribute.Value;
        }
    }
}
