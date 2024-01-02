using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Actor;
using SaloonSlingers.Unity.Tests;

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

        [Test]
        public void WhenHasEnemy_OnStart_UsesShieldHitPoints()
        {
            var attributes = TestUtils.CreateComponent<Attributes>();
            attributes.Registry[AttributeType.Health] = new Points(1);
            var enemy = attributes.gameObject.AddComponent<Enemy>();
            enemy.ShieldHitPoints = new Points(1);
            var task = new CheckHasShieldHitPoints
            {
                GameObject = enemy.gameObject
            };
            task.OnStart();

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
            enemy.ShieldHitPoints.Decrease(1);
            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));

        }
    }
}
