using System;
using System.Collections;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour, IActor
    {
        public event EventHandler Death;
        public Deck Deck { get; private set; }
        public Core.Attribute ShieldHitPoints { get; set; }
        public IDictionary<AttributeType, Core.Attribute> AttributeRegistry { get; private set; }

        [SerializeField]
        private GameObject shield;
        [SerializeField]
        private HoloShieldController holoShieldController;
        [SerializeField]
        private Collider _collider;

        private void OnEnable()
        {
            if (AttributeRegistry == null || !AttributeRegistry.TryGetValue(AttributeType.Health, out var hp))
                return;

            hp.Depleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            if (AttributeRegistry == null || !AttributeRegistry.TryGetValue(AttributeType.Health, out var hp))
                return;

            hp.Depleted -= OnHealthDepleted;
        }

        private void Awake()
        {
            Deck = new Deck().Shuffle();
            _collider ??= GetComponent<Collider>();
        }

        private void Start()
        {
            AttributeRegistry = GetComponent<Attributes>().Registry;
            if (AttributeRegistry.TryGetValue(AttributeType.Health, out var hp))
                hp.Depleted += OnHealthDepleted;

            ShieldHitPoints ??= holoShieldController?.HitPoints;
        }

        public void ResetActor()
        {
            AttributeRegistry[AttributeType.Health].Reset();
            ShieldHitPoints.Reset(0);
            _collider.enabled = true;
            Deck = new Deck().Shuffle();
        }

        public void Kill()
        {
            StartCoroutine(nameof(DoDeath));
        }

        private IEnumerator DoDeath()
        {
            _collider.enabled = false;
            yield return new WaitForSeconds(1f);
            Death?.Invoke(gameObject, EventArgs.Empty);
        }

        private void OnHealthDepleted(IReadOnlyAttribute sender, EventArgs e) => Kill();
    }
}
