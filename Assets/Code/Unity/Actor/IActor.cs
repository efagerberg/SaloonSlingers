using System;

namespace SaloonSlingers.Unity.Actor
{
    public interface IActor
    {
        public event EventHandler Death;
        public void ResetActor();
    }
}
