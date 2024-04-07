using System;
using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CoroutineTask
    {
        public bool Running { get; private set; } = false;

        private readonly Func<IEnumerator> generator;
        private readonly MonoBehaviour runner;

        public CoroutineTask(MonoBehaviour runner, Func<IEnumerator> generator)
        {
            this.generator = generator;
            this.runner = runner;
        }

        public void Run()
        {
            if (Running) return;

            runner.StartCoroutine(RunWrapped());
        }

        public void Stop()
        {
            if (!Running) return;

            runner.StopCoroutine(RunWrapped());
            Running = false;
        }

        public void Restart()
        {
            Stop();
            Run();
        }

        private IEnumerator RunWrapped()
        {
            Running = true;
            yield return generator();
            Running = false;
        }
    }
}
