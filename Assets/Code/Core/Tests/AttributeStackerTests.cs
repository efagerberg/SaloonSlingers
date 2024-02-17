using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class AttributeStackerTests
    {
        [TestCase(1, 2, 10, 12)]
        [TestCase(0, 10, 20, 10)]
        public void OnlyStacks_WhenStacksAvailable(int startingStacks, int startingAttributeValue, int increaseAmount, int expectedTotal)
        {
            var attributeToStack = new Attribute((uint)startingAttributeValue, uint.MaxValue);
            var stacker = new AttributeStacker((uint)startingStacks);
            stacker.Stack(attributeToStack, (uint)increaseAmount);

            Assert.That(attributeToStack.Value, Is.EqualTo((uint)expectedTotal));
        }

        [Test]
        public void WhenAttributeDepleted_StacksRestored()
        {
            var attributeToStack = new Attribute(0, uint.MaxValue);
            var stacker = new AttributeStacker(2);
            stacker.Stack(attributeToStack, 1);
            stacker.Stack(attributeToStack, 2);
            attributeToStack.Decrease(3);

            Assert.That(stacker.Stacks.Value, Is.EqualTo(stacker.Stacks.InitialValue));
        }

        [Test]
        public void CanStack_ReturnsTrue_OnlyIfStacksAvailable()
        {
            var stacker = new AttributeStacker(2);
            var attributeToStack = new Attribute(0, uint.MaxValue);
            stacker.Stack(attributeToStack, 1);
            Assert.That(stacker.CanStack);
            stacker.Stack(attributeToStack, 2);

            Assert.False(stacker.CanStack);
        }
    }
}
