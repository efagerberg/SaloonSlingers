using System;

namespace SaloonSlingers.Core
{
    /// <summary>
    /// Controls increase of an attribute based on if a number of stacks.
    /// Once the value is fully depleted a stack, all stacks are restored.
    /// </summary>
    public class AttributeStacker
    {
        private readonly Attribute stacks;

        public bool CanStack { get => stacks > 0; }
        public IReadOnlyAttribute Stacks { get => stacks; }

        public AttributeStacker(uint startingStacks)
        {
            stacks = new Attribute(startingStacks);
        }

        public void Stack(Attribute attribute, uint value)
        {
            if (!CanStack) return;

            attribute.Depleted += OnDepleted;
            stacks.Decrement();
            attribute.Reset(attribute + value);
        }

        private void OnDepleted(IReadOnlyAttribute sender, EventArgs e)
        {
            stacks.Reset();
            sender.Depleted -= OnDepleted;
        }
    }
}
