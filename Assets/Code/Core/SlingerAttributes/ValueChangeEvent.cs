using System;

namespace SaloonSlingers.Core
{
    public class ValueChangeEvent<T> : EventArgs
    {
        public readonly T Before;
        public readonly T After;

        public ValueChangeEvent(T before, T after) : base()
        {
            Before = before;
            After = after;
        }
    }
}
