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

        public void Absorb(Attribute absorbingAttribute, HandProjectile projectile)
        {
            if (!CanAbsorb) return;

            projectile.Pause();
            stacker.Stack(absorbingAttribute, projectile.HandEvaluation.Score);
            var actor = projectile.GetComponent<Actor.Actor>();
            actor.Kill();
        }
    }
}
