using System;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    class TestBehavior : MonoBehaviour
    {

    }

    public class PlayerDeathTests
    {
        [Test]
        [RequiresPlayMode]
        public void DoesNothing_WhenHitPointsRemaining()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);
            hitPoints.Points.Decrement();

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        [Test]
        [RequiresPlayMode]
        public void DoesNothing_WhenDisabled_ThenHitPointsReducedTo0()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);
            hitPoints.Points.Decrement();
            subject.enabled = false;
            hitPoints.Points.Decrease(2);

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        [Test]
        [RequiresPlayMode]
        public void StopsLocomotionAndLoadsGameOverScene_WhenHPReaches0()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);

            // GameManager will be null
            Assert.Throws<NullReferenceException>(() => hitPoints.Points.Decrease(hitPoints));
            Assert.That(subject.ComponentsToDisable.All(x => !x.enabled), Is.True);
        }
    }
}
