namespace SaloonSlingers.Core
{
    public class HandProjectileState
    {
        public bool IsAlive
        {
            get => lifespanInSeconds > 0;
        }
        public bool IsCommitted { get; private set; }
        public bool IsThrown { get; private set; }

        private float lifespanInSeconds;
        private readonly float originalLifeSpanInSeconds;

        public HandProjectileState(float lifespanInSeconds)
        {
            IsThrown = false;
            IsCommitted = false;
            this.lifespanInSeconds = lifespanInSeconds;
            originalLifeSpanInSeconds = lifespanInSeconds;
        }

        public HandProjectileState Throw()
        {
            IsThrown = true;
            return this;
        }

        public HandProjectileState ToggleCommit()
        {
            IsCommitted = !IsCommitted;
            return this;
        }

        public HandProjectileState Reset()
        {
            IsThrown = false;
            IsCommitted = false;
            lifespanInSeconds = originalLifeSpanInSeconds;
            return this;
        }

        public void Update(float updateTimeDelta)
        {
            if (IsThrown) lifespanInSeconds -= updateTimeDelta;
        }
    }
}
