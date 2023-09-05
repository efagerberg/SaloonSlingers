using System;

namespace SaloonSlingers.Unity
{
    public interface IActor
    {
        public event EventHandler Death;
        public void Reset();
    }
}
