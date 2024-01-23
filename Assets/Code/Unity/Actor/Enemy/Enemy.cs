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

        private void Awake()
        {
            Deck = new Deck().Shuffle();
        }

        private void Start()
        {
            AttributeRegistry = GetComponent<Attributes>().Registry;
            ShieldHitPoints ??= holoShieldController?.HitPoints;
        }

        public void ResetActor()
        {
            AttributeRegistry[AttributeType.Health].Reset();
            ShieldHitPoints.Reset(0);
            Deck = new Deck().Shuffle();
        }

        public void Kill()
        {
            StartCoroutine(nameof(DoDeath));
        }

        private IEnumerator DoDeath()
        {
            yield return new WaitForSeconds(1f);
            Death?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
