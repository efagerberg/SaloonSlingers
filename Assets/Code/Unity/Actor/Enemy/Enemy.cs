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
        public HitPoints HitPoints { get; private set; }
        public HitPoints ShieldHitPoints { get; private set; }

        [SerializeField]
        private GameObject shield;

        private void Awake()
        {
            HitPoints = GetComponent<HitPoints>();
            ShieldHitPoints = shield.GetComponent<HitPoints>();
            Deck = new Deck().Shuffle();
        }

        public void Reset()
        {
            HitPoints.Points.Reset();
            ShieldHitPoints.Points.Reset(0);
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
