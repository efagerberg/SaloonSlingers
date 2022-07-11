using System;

namespace SaloonSlingers.Core
{
    public class CardHandState
    {
        public bool IsAlive {
            get => !IsThrown || checkAlive();
        }
        public bool IsCommitted { get; private set; }
        public bool CanDraw { get => !IsCommitted && checkCanDraw(); }

        public bool IsThrown { get; private set; }
        private readonly Func<bool> checkAlive;
        private readonly Func<bool> checkCanDraw;

        public CardHandState(Func<bool> checkAlive, Func<bool> checkCanDraw)
        {
            IsThrown = false;
            IsCommitted = false;
            this.checkAlive = checkAlive;
            this.checkCanDraw = checkCanDraw;
        }

        public CardHandState Throw()
        {
            IsThrown = true;
            return this;
        }

        public CardHandState ToggleCommit()
        {
            IsCommitted = !IsCommitted;
            return this;
        }

        public CardHandState Reset()
        {
            IsThrown = false;
            IsCommitted = false;
            return this;
        }
    }
}
