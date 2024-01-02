using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class AttributePrimerTests
    {
        [Test]
        public void Errors_WhenGivenUnknownType()
        {
            var configs = new AttributeConfig[] {
                new() { Type = "A BAD TYPE" }
            };
            var root = new GameObject("Subject");

            Assert.Throws<InvalidAttributeError>(() =>
            {
                AttributePrimer.Prime(configs, root);
            });
        }

        [Test]
        public void AddsHitPoints_WhenTypeHealth()
        {
            AttributeConfig healthConfig = new() { Type = "health", Value = 3 };
            var configs = new AttributeConfig[] { healthConfig };
            var root = new GameObject("Subject");
            AttributePrimer.Prime(configs, root);

            Assert.That(root.TryGetComponent<Attributes>(out var attributes));
            Assert.That(attributes.Registry[AttributeType.Health].Value,
                        Is.EqualTo(healthConfig.Value));
        }

        [Test]
        public void AddsMoney_WhenTypeMoney()
        {
            AttributeConfig moneyConfig = new() { Type = "money", Value = 100 };
            var configs = new AttributeConfig[] { moneyConfig };
            var root = new GameObject("Subject");
            AttributePrimer.Prime(configs, root);

            Assert.That(root.TryGetComponent<Attributes>(out var attributes));
            Assert.That(attributes.Registry[AttributeType.Money].Value,
                        Is.EqualTo(moneyConfig.Value));
        }

        [Test]
        public void AddsDashable_WhenTypeDash()
        {
            AttributeConfig dashConfig = new()
            {
                Type = "dash",
                Value = 2,
                Cooldown = 0.1f,
                RecoveryPeriod = 3,
                Speed = 10,
                Duration = 0.4f
            };
            var expectedMetaData = new ActionMetaData()
            {
                Cooldown = dashConfig.Cooldown,
                RecoveryPeriod = dashConfig.RecoveryPeriod,
                Duration = dashConfig.Duration,
            };
            var configs = new AttributeConfig[] { dashConfig };
            var root = new GameObject("Subject");
            AttributePrimer.Prime(configs, root);

            Assert.That(root.TryGetComponent<Dashable>(out var dashable));
            Assert.That(dashable.Speed, Is.EqualTo(dashConfig.Speed));
            Assert.That(dashable.Points.Value, Is.EqualTo(dashConfig.Value));
            Assert.That(dashable.MetaData, Is.EqualTo(expectedMetaData));
        }

        [Test]
        public void AddsPeerable_WhenTypeDash()
        {
            AttributeConfig peerConfig = new()
            {
                Type = "peer",
                Value = 10,
                Cooldown = 1f,
                RecoveryPeriod = 3,
                Interval = 2,
                Duration = 2
            };
            var expectedMetaData = new ActionMetaData()
            {
                Cooldown = peerConfig.Cooldown,
                RecoveryPeriod = peerConfig.RecoveryPeriod,
                Duration = peerConfig.Duration,
            };
            var configs = new AttributeConfig[] { peerConfig };
            var root = new GameObject("Subject");
            AttributePrimer.Prime(configs, root);

            Assert.That(root.TryGetComponent<Peerable>(out var dashable));
            Assert.That(dashable.Interval, Is.EqualTo(peerConfig.Interval));
            Assert.That(dashable.Points.Value, Is.EqualTo(peerConfig.Value));
            Assert.That(dashable.MetaData, Is.EqualTo(expectedMetaData));
        }
    }
}
