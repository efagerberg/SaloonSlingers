using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Core;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    public class TestCheckHasHitPoints
    {
        [Test]
        public void WhenPointsSet_ButNoneAvailable_ReturnsFailure()
        {
            var task = new CheckHasShieldHitPoints()
            {
                HitPoints = new Points(0)
            };

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void WhenPointsSet_WithPointsAvailable_ReturnsSuccess()
        {
            var task = new CheckHasShieldHitPoints()
            {
                HitPoints = new Points(1)
            };

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }
    }
}
