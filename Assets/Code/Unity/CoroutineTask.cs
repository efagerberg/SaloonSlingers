using System;
using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CoroutineTask
    {
        private bool running = false;
        private readonly Func<IEnumerator> generator;
        private readonly MonoBehaviour runner;

        public CoroutineTask(MonoBehaviour runner, Func<IEnumerator> generator)
        {
            this.generator = generator;
            this.runner = runner;
        }

        public void Run()
        {
            if (running) return;

            runner.StartCoroutine(RunWrapped());
        }

        public void Stop()
        {
            if (!running) return;

            runner.StopCoroutine(RunWrapped());
            running = false;
        }

        public void Restart()
        {
            Stop();
            Run();
        }

        private IEnumerator RunWrapped()
        {
            running = true;
            yield return generator();
            running = false;
        }
    }
}
