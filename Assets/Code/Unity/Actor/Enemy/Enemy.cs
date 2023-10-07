using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour, IActor
    {
        public event EventHandler Death;

        private HitPoints hitPoints;

        public void Reset()
        {
            hitPoints.Points.Reset();
        }

        private void Awake()
        {
            hitPoints = GetComponent<HitPoints>();
        }

        private void OnEnable()
        {
            hitPoints.Points.PointsDecreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.PointsDecreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0) Death?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
