namespace SaloonSlingers.Core
{
    public class ThrowState
    {
        public bool IsAlive { get => !shouldDespawn; }
        private bool isThrown = false;
        private bool shouldDespawn;

        public ThrowState()
        {
            shouldDespawn = false;
        }

        public ThrowState Throw()
        {
            isThrown = true;
            return this;
        }

        public ThrowState Reset()
        {
            isThrown = false;
            shouldDespawn = false;
            return this;
        }

        public ThrowState Update(bool shouldDespawn)
        {
            if (!isThrown || !shouldDespawn) return this;
            this.shouldDespawn = shouldDespawn;
            return this;
        }
    }
}
