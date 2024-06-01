using System;
using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CoroutineTask
    {
        public bool Running { get; private set; } = false;
        private readonly MonoBehaviour runner;
        private Func<IEnumerator> currentCoroutineFunc;

        public CoroutineTask(MonoBehaviour runner)
        {
            this.runner = runner;
        }

        public void Run(Func<IEnumerator> coroutineFunc)
        {
            if (Running) return;

            currentCoroutineFunc = coroutineFunc;
            runner.StartCoroutine(RunWrapped());
        }

        public void Stop()
        {
            if (!Running) return;

            runner.StopCoroutine(RunWrapped());
            Running = false;
        }

        public void Restart(Func<IEnumerator> coroutineFunc = null)
        {
            Stop();
            Run(coroutineFunc ?? currentCoroutineFunc);
        }

        private IEnumerator RunWrapped()
        {
            Running = true;
            yield return currentCoroutineFunc();
            Running = false;
        }
    }
}
