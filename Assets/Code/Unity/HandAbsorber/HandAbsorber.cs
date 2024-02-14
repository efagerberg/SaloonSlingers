using System;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        public Core.Attribute Stacks { get; private set; }
        public bool CanAbsorb { get => Stacks > 0; }

        [SerializeField]
        private uint startingStacks = 3;

        private void Awake()
        {
            Stacks = new Core.Attribute(startingStacks);
        }

        public void Absorb(Core.Attribute shieldPoints, HandProjectile projectile)
        {
            if (!CanAbsorb) return;

            shieldPoints.Depleted += OnShieldBroken;
            Stacks.Decrement();

            projectile.Pause();
            shieldPoints.Reset(shieldPoints + projectile.HandEvaluation.Score);
            projectile.Kill();
        }

        private void OnShieldBroken(IReadOnlyAttribute sender, EventArgs e)
        {
            Stacks.Increment();
            sender.Depleted -= OnShieldBroken;
        }
    }
}
