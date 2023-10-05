using System.Collections.Generic;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class SaloonTests
    {
        [Test]
        public void Load_CreatesExpectedInstance()
        {
            var manifest = new Dictionary<string, int> {
                { "enemy1", 3 },
                { "enemy2", 2 }
            };
            var game = new CardGameConfig()
            {
                Name = "TestGame",
                HandEvaluator = "BlackJack"
            };
            var config = new SaloonConfig
            {
                Id = "TestSaloon",
                InterestRisk = 0.0f,
                EnemyManifest = manifest,
                HouseGame = game
            };
            var subject = Saloon.Load(config);

            Assert.That(subject.EnemyInventory.Manifest, Is.EqualTo(manifest));
            Assert.That(subject.InterestRisk, Is.EqualTo(config.InterestRisk));
            Assert.That(subject.HouseGame.Name, Is.EqualTo(config.HouseGame.Name));
            Assert.That(subject.Id, Is.EqualTo(config.Id));
        }
    }

    public class EnemyInventoryTests
    {
        [Test]
        public void WhenEnemyInventoryEmpty_IsComplete()
        {
            var manifest = new Dictionary<string, int>
            {
            };
            var subject = new EnemyInventory(manifest);

            Assert.That(subject.Completed, Is.True);
        }

        [Test]
        public void WhenNonRemaining_IsComplete()
        {
            var enemyManifest = new Dictionary<string, int>
            {
                { "enemy1", 1 },
                { "enemy2", 1 }
            };
            var subject = new EnemyInventory(enemyManifest);
            subject.RecordDeath(subject.GetRandomEnemy());
            subject.RecordDeath(subject.GetRandomEnemy());

            Assert.That(subject.Completed, Is.True);
        }

        [Test]
        public void GetsRandomEnemy()
        {
            var enemyManifest = new Dictionary<string, int>
            {
                { "enemy1", 1 },
                { "enemy2", 1 }
            };
            var subject = new EnemyInventory(enemyManifest);
            var enemy1 = subject.GetRandomEnemy();
            var enemy2 = subject.GetRandomEnemy();

            Assert.That(enemy1, Is.Not.EqualTo(enemy2));
        }

        [Test]
        public void Reset_ResetsCompletedInventoryToIncomplete()
        {
            var enemyManifest = new Dictionary<string, int>
            {
                { "enemy1", 1 },
                { "enemy2", 1 }
            };
            var subject = new EnemyInventory(enemyManifest);
            subject.RecordDeath(subject.GetRandomEnemy());
            subject.RecordDeath(subject.GetRandomEnemy());
            subject.Reset();

            Assert.That(subject.Completed, Is.False);
        }
    }
}

