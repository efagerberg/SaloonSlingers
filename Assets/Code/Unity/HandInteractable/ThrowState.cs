namespace SaloonSlingers.Unity.HandInteractable
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

        public void SetThrown() => isThrown = true;

        public void SetUnthrown()
        {
            timeToLive = maxLifetime;
            isThrown = false;
        }

        public void Update(float velocityMagnitude, float deltaTime)
        {
            if (!isThrown || shouldDespawn) return;
            if (velocityMagnitude == 0 || timeToLive <= 0) shouldDespawn = true;
            else timeToLive -= deltaTime;
        }
    }
}
