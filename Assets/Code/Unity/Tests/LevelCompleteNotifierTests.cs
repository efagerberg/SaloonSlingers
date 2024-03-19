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
        public void EmitsEven_WhenAllEnemiesKilled(int nEnemies)
        {
            var eventEmitted = false;
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
            void CompleteHandler() => eventEmitted = true;

            var subject = new LevelCompleteNotifier(manifest);
            subject.LevelCompleted.AddListener(CompleteHandler);
            for (int i = 0; i < nEnemies; i++)
            {
                actors[i].OnKilled.AddListener(subject.OnEnemyKilled);
                actors[i].Kill();
            }

            Assert.That(eventEmitted);
        }

        [Test]
        public void DoesNotEmitKillEventTwice_WhenEnemyKilled()
        {
            var eventEmitted = false;
            var enemy = new GameObject("TestEnemy");
            var actor = enemy.AddComponent<TestActor>();
            var manifest = new Dictionary<string, int>
            {
                { enemy.name, 2 }
            };
            void CompleeHandler() => eventEmitted = true;

            var subject = new LevelCompleteNotifier(manifest);
            actor.OnKilled.AddListener(subject.OnEnemyKilled);
            subject.LevelCompleted.AddListener(CompleeHandler);
            actor.Kill();
            actor.Kill();

            Assert.That(eventEmitted, Is.False);
        }
    }
}