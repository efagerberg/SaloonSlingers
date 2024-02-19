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
            var hitPoints = new Core.Attribute(10);
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = TestUtils.CreateComponent<PlayerDeath>();
            subject.HitPoints = hitPoints;
            yield return null;

            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            hitPoints.Increase(3);
            hitPoints.Decrement();

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator DoesNothing_WhenDisabled_ThenHitPointsReducedTo0()
        {
            var hitPoints = new Core.Attribute(2);
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = TestUtils.CreateComponent<PlayerDeath>();
            subject.HitPoints = hitPoints;
            yield return null;

            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";
            subject.enabled = false;
            hitPoints.Decrease(2);

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
            Assert.That(EditorSceneManager.GetActiveScene().name, Is.Not.EqualTo(subject.GameOverSceneName));
        }

        private static readonly bool[] deathTestInputs = { true, false };

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator StopsLocomotionAndLoadsGameOverScene_WhenHPReaches0([ValueSource(nameof(deathTestInputs))] bool checkWorksWhenComponentReset)
        {
            var hitPoints = new Core.Attribute(1);
            var toDisable = TestUtils.CreateComponent<TestBehavior>();
            var subject = TestUtils.CreateComponent<PlayerDeath>();
            subject.HitPoints = hitPoints;
            yield return null;

            if (checkWorksWhenComponentReset)
            {
                subject.enabled = false;
                subject.enabled = true;
            }

            subject.ComponentsToDisable = new Behaviour[] { toDisable };
            subject.GameOverSceneName = "A fake scene";

            // GameManager will be null
            Assert.Throws<NullReferenceException>(() => hitPoints.Decrease(hitPoints));
            Assert.That(subject.ComponentsToDisable.All(x => !x.enabled), Is.True);
        }

        [Test]
        public void EnablesBehaviors_WhenActorReset()
        {
            var hitPoints = new Core.Attribute(10);
            var disabled = TestUtils.CreateComponent<TestBehavior>();
            disabled.enabled = false;
            var subject = TestUtils.CreateComponent<PlayerDeath>();
            subject.ComponentsToDisable = new Behaviour[] { disabled };
            subject.ResetActor();

            Assert.That(subject.ComponentsToDisable.All(x => x.enabled), Is.True);
        }
    }
}
