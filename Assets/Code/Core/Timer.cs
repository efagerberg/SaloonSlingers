
using System;

namespace SaloonSlingers.Core
{
    public struct Timer
    {
        public float DurationInSeconds;

        private float originalDurationInSeconds;

        public Timer(float duration)
        {
            DurationInSeconds = duration;
            originalDurationInSeconds = duration;
        }

        public bool CheckPassed(float deltaTime)
        {
            DurationInSeconds = MathF.Max(0, DurationInSeconds - deltaTime);
            if (DurationInSeconds > 0) return false;

            return true;
        }

        public void Reset(float? newDuration = null)
        {
            if (newDuration == null)
                DurationInSeconds = originalDurationInSeconds;
            else
            {
                DurationInSeconds = newDuration.Value;
                originalDurationInSeconds = newDuration.Value;
            }
        }
    }
}