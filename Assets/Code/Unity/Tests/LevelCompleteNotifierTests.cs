using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class LevelCompleteNotifierTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void Emits_WhenAllEnemiesKilled(int nEnemies)
        {
            LevelResult levelResult = LevelResult.UNDEFINED;
            GameObject[] enemies = new GameObject[nEnemies];
            TestActor[] actors = new TestActor[nEnemies];

            for (int i = 0; i < nEnemies; i++)
            {
                var enemy = new GameObject("TestEnemy");
                enemies[i] = enemy;
                var actor = enemy.AddComponent<TestActor>();
                actors[i] = actor;
            }

            var manifest = new Dictionary<string, int>
            {
                { enemies[0].name, nEnemies }
            };
            void CompletedHandler(LevelResult r) => levelResult = r;

            var subject = new LevelCompleteNotifier(manifest);
            subject.LevelCompleted.AddListener(CompletedHandler);
            for (int i = 0; i < nEnemies; i++)
            {
                actors[i].OnKilled.AddListener(subject.OnEnemyKilled);
                actors[i].Kill();
            }

            Assert.That(levelResult, Is.EqualTo(LevelResult.ALL_ENEMIES_KILLED));
        }

        [Test]
        public void DoesNotEmitKillEventTwice_WhenEnemyKilled()
        {
            LevelResult levelResult = LevelResult.UNDEFINED;
            var enemy = new GameObject("TestEnemy");
            var actor = enemy.AddComponent<TestActor>();
            var manifest = new Dictionary<string, int>
            {
                { enemy.name, 2 }
            };
            void CompletedHandler(LevelResult r) => levelResult = r;

            var subject = new LevelCompleteNotifier(manifest);
            actor.OnKilled.AddListener(subject.OnEnemyKilled);
            subject.LevelCompleted.AddListener(CompletedHandler);
            actor.Kill();
            actor.Kill();

            Assert.That(levelResult, Is.EqualTo(LevelResult.UNDEFINED));
        }

        [Test]
        public void Emits_WhenPlayerKilled()
        {
            LevelResult levelResult = LevelResult.UNDEFINED;

            var player = new GameObject("TestPlayer");
            var actor = player.AddComponent<TestActor>();
            var manifest = new Dictionary<string, int> { };

            void CompletedHandler(LevelResult r) => levelResult = r;

            var subject = new LevelCompleteNotifier(manifest);
            actor.OnKilled.AddListener(subject.OnPlayerKilled);
            subject.LevelCompleted.AddListener(CompletedHandler);
            actor.Kill();

            Assert.That(levelResult, Is.EqualTo(LevelResult.PLAYER_KILLED));
        }
    }
}