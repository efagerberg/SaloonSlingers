using System;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class Pickup : MonoBehaviour, IActor
    {
        public uint Value;
        public event EventHandler Killed;

        public void ResetActor()
        {
            Value = 0;
        }

        public void Kill()
        {
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
