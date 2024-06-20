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

        public void Absorb(Attribute absorbingAttribute, CardHand hand)
        {
            if (!CanAbsorb) return;

            hand.Pause();
            stacker.Stack(absorbingAttribute, hand.Evaluation.Score);
            var actor = hand.GetComponent<Actor.Actor>();
            actor.Kill();
        }
    }
}
