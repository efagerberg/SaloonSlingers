
using System;

namespace SaloonSlingers.Core
{
    public struct Timer
    {
        public float Remaining { get; private set; }
        public float Elapsed { get; private set; }
        public float Duration { get; private set; }

        public Timer(float? duration)
        {
            Duration = duration.Value;
            Elapsed = 0f;
            Remaining = Duration;
        }

        public bool Tick(float deltaTime)
        {
            Remaining = MathF.Max(0, Remaining - deltaTime);
            Elapsed += deltaTime;
            return Remaining <= 0;
        }

        public void Reset(float? newDuration = null)
        {
            Elapsed = 0f;
            if (newDuration == null)
                Remaining = Duration;
            else
            {
                Remaining = newDuration.Value;
                Duration = newDuration.Value;
            }
        }
    }
}