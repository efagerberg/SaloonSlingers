using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour, IActor
    {
        public event EventHandler Death;
        public Deck Deck { get; private set; }

        private HitPoints hitPoints;

        private void Awake()
        {
            hitPoints = GetComponent<HitPoints>();
            Deck = new Deck().Shuffle();
    }

        public void Reset()
        {
            hitPoints.Points.Reset();
            Deck = new Deck().Shuffle();
        }

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
