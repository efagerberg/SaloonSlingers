using UnityEngine;
using UnityEngine.XR;

namespace SaloonSlingers.Unity.Actor
{
    public enum Handedness
    {
        RIGHT, LEFT, NONE
    }

    public class ActorHandedness : MonoBehaviour
    {
        public Handedness Current
        {
            get => _current; private set => _current = value;
        }

        public DeckGraphic DeckGraphic;
        public EnemyHandDisplay EnemyPeerDisplay;

        [SerializeField]
        private Handedness _current = Handedness.NONE;

        private void OnEnable() => InputDevices.deviceConnected += SetHandedness;

        private void OnDisable() => InputDevices.deviceConnected -= SetHandedness;

        private void SetHandedness(InputDevice device) => Current = GetHandedness(device);

        private Handedness GetHandedness(InputDevice device)
        {
            if (Current != Handedness.NONE) return Current;
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out _))
            {
                switch (device.name.ToLower())
                {
                    case "left":
                        return Handedness.LEFT;
                    case "right":
                        return Handedness.RIGHT;
                }
            }
            return Handedness.RIGHT;
        }
    }
}
