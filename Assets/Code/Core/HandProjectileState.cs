namespace SaloonSlingers.Core
{
    public class HandProjectileState
    {
        public bool IsAlive
        {
            get => lifespanInSeconds > 0;
        }
        public bool IsStacked { get; private set; }
        public bool IsThrown { get; private set; }

        private float lifespanInSeconds;
        private readonly float originalLifeSpanInSeconds;

        public HandProjectileState(float lifespanInSeconds)
        {
            IsThrown = false;
            IsStacked = false;
            this.lifespanInSeconds = lifespanInSeconds;
            originalLifeSpanInSeconds = lifespanInSeconds;
        }

        public HandProjectileState Throw()
        {
            IsThrown = true;
            return this;
        }

        public HandProjectileState Pause()
        {
            IsThrown = false;
            return this;
        }

        public HandProjectileState Stack()
        {
            IsStacked = true;
            return this;
        }

        public HandProjectileState Unstack()
        {
            IsStacked = false;
            return this;
        }

        public HandProjectileState Reset()
        {
            IsThrown = false;
            IsStacked = false;
            lifespanInSeconds = originalLifeSpanInSeconds;
            return this;
        }

        public void Update(float updateTimeDelta)
        {
            if (IsThrown) lifespanInSeconds -= updateTimeDelta;
        }
    }
}
