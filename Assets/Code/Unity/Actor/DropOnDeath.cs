using System;

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
            if (!attributes.Registry.TryGetValue(AttributeType.Pot, out var pot) ||
                !attributes.Registry.TryGetValue(AttributeType.Money, out var money)) return;

            var spawnedPickup = LevelManager.Instance.PickupSpawner.Spawn();
            spawnedPickup.layer = gameObject.layer;
            spawnedPickup.transform.position = DropPositionReference.position;
            float scaleFactor = 1 + pot.Value / money.Value;
            spawnedPickup.transform.localScale = Vector3.one * scaleFactor;
            var pickup = spawnedPickup.GetComponent<Pickup>();
            pickup.Value = pot;
        }
    }
}
