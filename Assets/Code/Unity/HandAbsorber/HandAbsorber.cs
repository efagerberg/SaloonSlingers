using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        public Attribute Stacks { get; private set; }
        public bool CanAbsorb { get => Stacks > 0; }

        [SerializeField]
        private uint startingStacks = 3;

        private void Awake()
        {
            Stacks = new Attribute(startingStacks);
        }

        public void Absorb(Attribute points, HandProjectile projectile)
        {
            if (!CanAbsorb) return;

            points.Decreased += CheckStackRegained;
            Stacks.Decrement();

            projectile.Pause();
            points.Reset(points + projectile.HandEvaluation.Score);
            projectile.Kill();
        }

        private void CheckStackRegained(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            Stacks.Increment();
            sender.Decreased -= CheckStackRegained;
        }
    }
}
