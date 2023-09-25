using System.Collections.Generic;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HeistTests
    {
        [Test]
        public void TestLoadCreatesExpectedInstance()
        {
            var enemyManifest = new Dictionary<string, int> {
                { "enemy1", 3 },
                { "enemy2", 2 }
            };
            var game = new CardGameConfig()
            {
                Name = "TestGame",
                HandEvaluator = "BlackJack"
            };
            var config = new HeistConfig
            {
                SaloonId = "TestSaloon",
                InterestRisk = 0.0f,
                EnemyInventory = enemyManifest,
                HouseGame = game
            };
            var subject = Heist.Load(config);

            Assert.That(subject.EnemyInventory.Manifest, Is.EqualTo(enemyManifest));
            Assert.That(subject.InterestRisk, Is.EqualTo(config.InterestRisk));
            Assert.That(subject.HouseGame.Name, Is.EqualTo(config.HouseGame.Name));
            Assert.That(subject.SaloonId, Is.EqualTo(config.SaloonId));
        }
    }

    public class EnemyInventoryTests
    {
        [Test]
        public void TestCompleteWhenEnemyInventoryEmpty()
        {
            var subject = new EnemyInventory();

            Assert.That(subject.Completed, Is.True);
        }

        [Test]
        public void TestCompleteWhenNonRemaining()
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
        public void TestGetsRandomEnemy()
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
        public void TestResetResetsCompletedInventoryToIncomplete()
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

