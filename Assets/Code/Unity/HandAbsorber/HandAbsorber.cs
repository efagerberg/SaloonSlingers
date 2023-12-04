using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        public Points Stacks { get; private set; }
        public bool CanAbsorb { get => Stacks > 0; }

        [SerializeField]
        private uint startingStacks = 3;

        private void Awake()
        {
            Stacks = new Points(startingStacks);
        }

        public void Absorb(HitPoints hitPoints, HandProjectile projectile)
        {
            if (!CanAbsorb) return;

            hitPoints.Points.Decreased += CheckStackRegained;
            Stacks.Decrement();

            projectile.Pause();
            hitPoints.Points.Reset(hitPoints + projectile.HandEvaluation.Score);
            projectile.Kill();
        }

        private void CheckStackRegained(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            Stacks.Increment();
            sender.Decreased -= CheckStackRegained;
        }
    }
}
