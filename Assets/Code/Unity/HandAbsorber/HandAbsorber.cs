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

        public void Absorb(Points points, HandProjectile projectile)
        {
            if (!CanAbsorb) return;

            points.Decreased += CheckStackRegained;
            Stacks.Decrement();

            projectile.Pause();
            points.Reset(points + projectile.HandEvaluation.Score);
            projectile.Kill();
        }

        private void CheckStackRegained(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            Stacks.Increment();
            sender.Decreased -= CheckStackRegained;
        }
    }
}
