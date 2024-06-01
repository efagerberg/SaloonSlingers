using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class TestBehaviour : MonoBehaviour { }

    public class CoroutineTaskTests
    {
        [Test]
        public void Run_OnlyRunsOnce_WhenCalledTwiceAndRunningAlready()
        {
            int runCount = 0;
            IEnumerator generator()
            {
                runCount++;
                yield return null;
            }
            var runner = TestUtils.CreateComponent<TestBehaviour>();
            var subject = new CoroutineTask(runner);
            subject.Run(generator);
            subject.Run(generator);

            Assert.That(subject.Running);
            Assert.That(runCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator Stop_CancelsCoroutine()
        {
            int runCount = 0;
            IEnumerator generator()
            {
                runCount++;
                yield return null;
                yield return null;
            }
            var runner = TestUtils.CreateComponent<TestBehaviour>();
            var subject = new CoroutineTask(runner);
            subject.Run(generator);
            yield return null;
            subject.Stop();

            Assert.That(!subject.Running);
            Assert.That(runCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator Restart_CancelsAndReRuns()
        {
            int runCount = 0;
            IEnumerator generator()
            {
                runCount++;
                yield return null;
                yield return null;
            }
            var runner = TestUtils.CreateComponent<TestBehaviour>();
            var subject = new CoroutineTask(runner);
            subject.Run(generator);
            yield return null;

            foreach (var passGeneratorToRestart in new bool[] { false, true })
            {
                if (passGeneratorToRestart)
                    subject.Restart(generator);
                else
                    subject.Restart();
                yield return null;
            }

            Assert.That(subject.Running);
            Assert.That(runCount, Is.EqualTo(3));
        }
    }
}
