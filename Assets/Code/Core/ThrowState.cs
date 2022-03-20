namespace SaloonSlingers.Core
{
    public class ThrowState
    {
        public bool IsAlive { get => !shouldDespawn; }
        private float timeToLive;
        private readonly float maxLifetime;
        private bool isThrown = false;
        private bool shouldDespawn;

        public ThrowState(float maxLifetime)
        {
            this.maxLifetime = maxLifetime;
            timeToLive = maxLifetime;
            shouldDespawn = false;
        }

        public ThrowState Throw()
        {
            isThrown = true;
            return this;
        }

        public ThrowState Reset()
        {
            timeToLive = maxLifetime;
            isThrown = false;
            return this;
        }

        public ThrowState Update(float velocityMagnitude, float deltaTime)
        {
            if (!isThrown || shouldDespawn) return this;
            timeToLive -= deltaTime;
            if (velocityMagnitude == 0 || timeToLive <= 0) shouldDespawn = true;
            return this;
        }
    }
}
