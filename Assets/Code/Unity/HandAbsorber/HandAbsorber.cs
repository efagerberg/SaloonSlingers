using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        public IReadOnlyAttribute Stacks { get => stacker.Stacks; }
        public bool CanAbsorb { get => stacker.CanStack; }

        [SerializeField]
        private uint startingStacks = 3;

        private AttributeStacker stacker;

        private void Awake()
        {
            stacker = new AttributeStacker(startingStacks);
        }

        public void Absorb(Attribute shieldPoints, HandProjectileActor projectile)
        {
            if (!CanAbsorb) return;

            projectile.Pause();
            stacker.Stack(shieldPoints, projectile.HandEvaluation.Score);
            projectile.Kill();
        }
    }
}
