using System;

namespace SaloonSlingers.Core
{
    public class HandInteractableState
    {
        public bool IsAlive {
            get => !IsThrown || checkAlive();
        }
        public bool IsCommitted { get; private set; }
        public bool CanDraw { get => !IsCommitted && checkCanDraw(); }

        public bool IsThrown { get; private set; }
        private readonly Func<bool> checkAlive;
        private readonly Func<bool> checkCanDraw;

        public HandInteractableState(Func<bool> checkAlive, Func<bool> checkCanDraw)
        {
            IsThrown = false;
            IsCommitted = false;
            this.checkAlive = checkAlive;
            this.checkCanDraw = checkCanDraw;
        }

        public HandInteractableState Throw()
        {
            IsThrown = true;
            return this;
        }

        public HandInteractableState ToggleCommit()
        {
            IsCommitted = !IsCommitted;
            return this;
        }

        public HandInteractableState Reset()
        {
            IsThrown = false;
            IsCommitted = false;
            return this;
        }
    }
}
