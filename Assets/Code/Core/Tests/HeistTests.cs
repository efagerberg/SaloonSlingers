using System.Collections.Generic;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HeistTests
    {
        [Test]
        public void TestIncompleteWhenEnemiesLeftInInventory()
        {
            var enemyManifest = new Dictionary<string, int> {
                { "enemy1", 3 },
                { "enemy2", 2 }
            };
            var games = new CardGameConfig[] {
                new CardGameConfig() {
                    Name="Game1",
                    HandEvaluator="BlackJack"
                },
                new CardGameConfig() {
                    Name="Game2",
                    HandEvaluator="Poker"
                },
            };
            var config = new HeistConfig
            {
                SaloonId = "TestSaloon",
                InterestRisk = 0.0f,
                EnemyInventory = enemyManifest,
                HouseGames = games
            };
            var subject = Heist.Load(config);

            Assert.That(subject.Complete, Is.False);
        }

        [Test]
        public void TestCompleteWhenEnemyInventoryEmpty()
        {
            var enemyManifest = new Dictionary<string, int>();
            var games = new CardGameConfig[] {
                new CardGameConfig() {
                    Name="Game1",
                    HandEvaluator="BlackJack"
                },
                new CardGameConfig() {
                    Name="Game2",
                    HandEvaluator="Poker"
                },
            };
            var config = new HeistConfig
            {
                SaloonId = "TestSaloon",
                InterestRisk = 0.0f,
                EnemyInventory = enemyManifest,
                HouseGames = games
            };
            var subject = Heist.Load(config);

            Assert.That(subject.Complete, Is.True);
        }

        [Test]
        public void TestCompleteWhenEnemyInventoryBecomesEmpty()
        {
            var enemyManifest = new Dictionary<string, int>
            {
                { "enemy1", 1 },
                { "enemy2", 1 }
            };
            var games = new CardGameConfig[] {
                new CardGameConfig() {
                    Name="Game1",
                    HandEvaluator="BlackJack"
                },
                new CardGameConfig() {
                    Name="Game2",
                    HandEvaluator="Poker"
                },
            };
            var config = new HeistConfig
            {
                SaloonId = "TestSaloon",
                InterestRisk = 0.0f,
                EnemyInventory = enemyManifest,
                HouseGames = games
            };
            var subject = Heist.Load(config);
            subject.GetRandomEnemy();
            subject.GetRandomEnemy();

            Assert.That(subject.Complete, Is.True);
        }

        [Test]
        public void TestGetsRandomEnemy()
        {
            var enemyManifest = new Dictionary<string, int>
            {
                { "enemy1", 1 },
                { "enemy2", 1 }
            };
            var games = new CardGameConfig[] {
                new CardGameConfig() {
                    Name="Game1",
                    HandEvaluator="BlackJack"
                },
                new CardGameConfig() {
                    Name="Game2",
                    HandEvaluator="Poker"
                },
            };
            var config = new HeistConfig
            {
                SaloonId = "TestSaloon",
                InterestRisk = 0.0f,
                EnemyInventory = enemyManifest,
                HouseGames = games
            };
            var subject = Heist.Load(config);
            var enemy1 = subject.GetRandomEnemy();
            var enemy2 = subject.GetRandomEnemy();

            Assert.That(enemy1, Is.Not.EqualTo(enemy2));
        }
    }
}

