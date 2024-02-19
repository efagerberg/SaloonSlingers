using System;

namespace SaloonSlingers.Unity.Actor
{
    public interface IActor
    {
        public event EventHandler Killed;
        public void ResetActor() { }
    }
}
