using System;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class DropPickupOnDeath : MonoBehaviour
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
}
