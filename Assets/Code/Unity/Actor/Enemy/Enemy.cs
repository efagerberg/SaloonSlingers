using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour, IActor
    {
        public event EventHandler Death;
        public Deck Deck { get; private set; }
        public HitPoints HitPoints { get; private set; }
        public HitPoints shieldHitPoints { get; private set; }

        [SerializeField]
        private GameObject shield;

        private void Awake()
        {
            HitPoints = GetComponent<HitPoints>();
            shieldHitPoints = shield.GetComponent<HitPoints>();
            Deck = new Deck().Shuffle();
        }

        public void Reset()
        {
            HitPoints.Points.Reset();
            shieldHitPoints.Points.Reset(0);
            Deck = new Deck().Shuffle();
        }

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
