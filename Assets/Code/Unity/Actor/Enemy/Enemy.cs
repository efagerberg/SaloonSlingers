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
        public TemporaryHitPoints TemporaryHitPoints { get; private set; }

        private void Awake()
        {
            HitPoints = GetComponent<HitPoints>();
            TemporaryHitPoints = GetComponent<TemporaryHitPoints>();
            Deck = new Deck().Shuffle();
        }

        public void Reset()
        {
            HitPoints.Points.Reset();
            TemporaryHitPoints.Points.Reset(0);
            Deck = new Deck().Shuffle();
        }

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
