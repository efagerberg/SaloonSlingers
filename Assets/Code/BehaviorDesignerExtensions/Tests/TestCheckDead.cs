using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Core;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    public class TestCheckDead
    {
        [Test]
        public void WhenHealthPointsSame_ReturnFailure()
        {
            var task = new CheckDead
            {
                HitPoints = new Points(1)
            };

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHealthPointsDecreasedButNot0_ReturnFailure()
        {
            var hitPoints = new Points(2);
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
            var hitPoints = new Points(2);
            var task = new CheckDead()
            {
                HitPoints = hitPoints
            };
            hitPoints.Decrease(2);

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }
    }
}
