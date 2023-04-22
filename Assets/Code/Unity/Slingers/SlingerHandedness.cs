using SaloonSlingers.Unity.CardEntities;

using UnityEngine;
using UnityEngine.XR;

public enum Handedness
{
    RIGHT, LEFT
}

public class SlingerHandedness : MonoBehaviour
{
    public Handedness Current
    {
        get; private set;
    }

    public DeckGraphic DeckGraphic;

    [SerializeField]
    private Handedness defaultHandedness = Handedness.RIGHT;

    private void OnEnable() => InputDevices.deviceConnected += SetHandedness;

    private void OnDisable() => InputDevices.deviceConnected -= SetHandedness;

    private void SetHandedness(InputDevice device) => Current = GetHandedness(device);

    private Handedness GetHandedness(InputDevice device)
    {
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
        return defaultHandedness;
    }
}
