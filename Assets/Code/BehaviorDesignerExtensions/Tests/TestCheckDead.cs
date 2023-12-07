using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;
using SaloonSlingers.Unity.Tests;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    public class TestCheckDead
    {
        [Test]
        public void WhenHealthPointsSame_ReturnFailure()
        {
            var task = new CheckDead();
            task.HitPoints = TestUtils.CreateComponent<HitPoints>();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHealthPointsDecreasedButNot0_ReturnFailure()
        {
            var task = new CheckDead();
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            hitPoints.Points.Reset(10);
            task.HitPoints = hitPoints;
            hitPoints.Points.Decrement();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHealthPointsDecreasedTo0_ReturnsSuccess()
        {
            var task = new CheckDead();
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            hitPoints.Points.Reset(1);
            task.HitPoints = hitPoints;
            hitPoints.Points.Decrement();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }
    }
}
