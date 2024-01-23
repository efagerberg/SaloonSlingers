using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Tests;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    public class TestCheckDead
    {
        [Test]
        public void WhenHealthPointsSame_ReturnFailure()
        {
            var task = new CheckDead
            {
                HitPoints = new Attribute(1)
            };

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHealthPointsDecreasedButNot0_ReturnFailure()
        {
            var hitPoints = new Attribute(2);
            var task = new CheckDead
            {
                HitPoints = hitPoints
            };
            hitPoints.Decrement();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHealthPointsDecreasedTo0_ReturnsSuccess()
        {
            var hitPoints = new Attribute(2);
            var task = new CheckDead()
            {
                HitPoints = hitPoints
            };
            hitPoints.Decrease(2);

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }

        [Test]
        public void WhenHasAttributes_OnAwake_UsesAttributeHealth()
        {
            var attributes = TestUtils.CreateComponent<Attributes>();
            var points = new Attribute(1);
            attributes.Registry[AttributeType.Health] = points;
            var task = new CheckDead
            {
                GameObject = attributes.gameObject
            };
            task.OnAwake();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
            points.Decrease(1);
            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));

        }
    }
}
