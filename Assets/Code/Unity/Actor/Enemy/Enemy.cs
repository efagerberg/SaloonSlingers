using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour, IActor
    {
        public event EventHandler Death;
        public Deck Deck { get; private set; }
        public Points ShieldHitPoints { get; set; }

        [SerializeField]
        private GameObject shield;
        [SerializeField]
        private HoloShieldController holoShieldController;

        private Points HitPoints { get; set; }

        private void Awake()
        {
            Deck = new Deck().Shuffle();
        }

        private void Start()
        {
            HitPoints = GetComponent<Attributes>().Registry[AttributeType.Health];
            ShieldHitPoints ??= holoShieldController?.HitPoints;
        }

        public void Reset()
        {
            HitPoints?.Reset();
            ShieldHitPoints?.Reset(0);
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
