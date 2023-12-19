using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Tests;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Actor.Tests
{
    public class ActionPerformerTests
    {
        private static readonly ActionMetaData metaData = new()
        {
            Duration = 0.1f,
            Cooldown = 0.1f,
            RecoveryPeriod = 0.2f
        };

        [Test]
        public void WhenNoPoints_ReturnsNoCoroutine()
        {
            Points points = new(0);

            static IEnumerator testCoroutine()
            {
                yield return new WaitForSeconds(metaData.Duration);
            }
            var subject = TestUtils.CreateComponent<ActionPerformer>();
            subject.Initialize(points, metaData);
            var actionCoroutine = subject.GetActionCoroutine(testCoroutine);

            Assert.That(actionCoroutine, Is.Null);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator WhenPoints_PerformsAction()
        {
            uint originalPointCount = 2;
            Points points = new(originalPointCount);
            bool ranCoroutine = false;
            IEnumerator testCoroutine()
            {
                ranCoroutine = true;
                yield return new WaitForSeconds(metaData.Duration);
            }
            var subject = TestUtils.CreateComponent<ActionPerformer>();
            subject.Initialize(points, metaData);
            var actionCoroutine = subject.GetActionCoroutine(testCoroutine);
            subject.StartCoroutine(actionCoroutine);

            Assert.That(actionCoroutine, Is.Not.Null);
            yield return new WaitForSeconds(metaData.Duration / 2f);
            Assert.That(ranCoroutine, Is.True);
            Assert.That(points.Value, Is.EqualTo(originalPointCount - 1));
            yield return new WaitForSeconds((metaData.Duration / 2f) + metaData.RecoveryPeriod);
            Assert.That(points.Value, Is.EqualTo(originalPointCount));
        }

        [Test]
        public void WhenAlreadyRunning_ReturnsNoCoroutine()
        {
            uint originalPointCount = 2;
            Points points = new(originalPointCount);
            static IEnumerator testCoroutine()
            {
                yield return new WaitForSeconds(metaData.Duration);
            }
            var subject = TestUtils.CreateComponent<ActionPerformer>();
            subject.Initialize(points, metaData);
            var actionCoroutine = subject.GetActionCoroutine(testCoroutine);
            subject.StartCoroutine(actionCoroutine);
            var secondCoroutine = subject.GetActionCoroutine(testCoroutine);

            Assert.That(actionCoroutine, Is.Not.Null);
            Assert.That(secondCoroutine, Is.Null);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator WhenRunTwice_BeforeCooldown_OnlyGeneratesOneCoroutine()
        {
            uint originalPointCount = 2;
            Points points = new(originalPointCount);
            static IEnumerator testCoroutine()
            {
                yield return new WaitForSeconds(metaData.Duration);
            }
            var subject = TestUtils.CreateComponent<ActionPerformer>();
            subject.Initialize(points, metaData);
            var actionCoroutine = subject.GetActionCoroutine(testCoroutine);
            subject.StartCoroutine(actionCoroutine);
            yield return new WaitForSeconds(metaData.Cooldown / 2f);
            var secondCoroutine = subject.GetActionCoroutine(testCoroutine);

            Assert.That(actionCoroutine, Is.Not.Null);
            Assert.That(secondCoroutine, Is.Null);
        }
    }
}
