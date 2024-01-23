using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Core;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    public class TestCheckHealthDamaged
    {
        [Test]
        public void WhenHitPointsSame_ReturnsFailure()
        {
            var task = new CheckHealthDamaged()
            {
                HitPoints = new Attribute(1)
            };
            task.OnAwake();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHitPointsDecreased_ReturnsSuccess()
        {
            var hitPoints = new Attribute(2);
            var task = new CheckHealthDamaged() { HitPoints = hitPoints };
            task.OnAwake();
            hitPoints.Decrement();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }


        [Test]
        public void WhenHitPointsDecreased_ThenUpdated_ReturnsFailure()
        {
            var hitPoints = new Attribute(2);
            var task = new CheckHealthDamaged() { HitPoints = hitPoints };
            task.OnAwake();
            hitPoints.Decrement();
            task.OnUpdate();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }
    }
}
