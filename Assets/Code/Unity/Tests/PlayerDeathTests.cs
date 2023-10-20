using System;
using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEditor.SceneManagement;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class PlayerDeathTests
    {
        [Test]
        [RequiresPlayMode]
        public void DoesNothing_WhenHitPointsRemaining()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);
            hitPoints.Points.Decrement();

            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        [Test]
        [RequiresPlayMode]
        public void DoesNothing_WhenDisabled_ThenHitPointsReducedTo0()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);
            hitPoints.Points.Decrement();
            subject.enabled = false;
            hitPoints.Points.Decrease(2);

            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        [Test]
        [RequiresPlayMode]
        public void TriesToLoadGameOverScene_WhenHPReaches0()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);

            // GameManager will be null
            Assert.Throws<NullReferenceException>(() => hitPoints.Points.Decrease(hitPoints.Points.Value));
        }
    }
}
