using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;
using SaloonSlingers.Unity.Tests;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    public class TestCheckHealthDamaged
    {
        [Test]
        public void WhenHitPointsSame_ReturnsFailure()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var task = new CheckHealthDamaged();
            task.HitPoints = hitPoints;
            task.OnAwake();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenHitPointsDecreased_ReturnsSuccess()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var task = new CheckHealthDamaged();
            task.HitPoints = hitPoints;
            task.OnAwake();
            hitPoints.Points.Decrement();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }


        [Test]
        public void WhenHitPointsDecreased_ThenUpdated_ReturnsFailure()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var task = new CheckHealthDamaged();
            task.HitPoints = hitPoints;
            task.OnAwake();
            hitPoints.Points.Decrement();
            task.OnUpdate();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }
    }
}
