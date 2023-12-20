using System;
using System.Collections;
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
        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator DoesNothing_WhenHitPointsRemaining()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            yield return null;

            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);
            hitPoints.Points.Decrement();

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator DoesNothing_WhenDisabled_ThenHitPointsReducedTo0()
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            yield return null;

            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);
            hitPoints.Points.Decrement();
            subject.enabled = false;
            hitPoints.Points.Decrease(2);

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        private static readonly bool[] deathTestInputs = { true, false };

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator StopsLocomotionAndLoadsGameOverScene_WhenHPReaches0([ValueSource(nameof(deathTestInputs))] bool checkWorksWhenComponentReset)
        {
            var hitPoints = TestUtils.CreateComponent<HitPoints>();
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = hitPoints.gameObject.AddComponent<PlayerDeath>();
            yield return null;

            if (checkWorksWhenComponentReset)
            {
                subject.enabled = false;
                subject.enabled = true;
            }

            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Points.Increase(3);

            // GameManager will be null
            Assert.Throws<NullReferenceException>(() => hitPoints.Points.Decrease(hitPoints));
            Assert.That(subject.ComponentsToDisable.All(x => !x.enabled), Is.True);
        }
    }
}
