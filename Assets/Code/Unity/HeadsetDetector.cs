using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace SaloonSlingers.Unity
{
    public class HeadsetDetector : MonoBehaviour
    {
        private InputDeviceCharacteristics headsetCharacteristic = InputDeviceCharacteristics.HeadMounted;
        private XRDeviceSimulator simulator;

        private void Awake()
        {
            simulator = GetComponent<XRDeviceSimulator>();
            simulator.enabled = true;
        }

        private void OnEnable()
        {
            InputDevices.deviceConnected += TurnOffSimulatorIfHMD;
            InputDevices.deviceDisconnected += TurnOnSimulatorIfHMD;
        }

        private void OnDisable()
        {
            InputDevices.deviceConnected -= TurnOffSimulatorIfHMD;
            InputDevices.deviceDisconnected -= TurnOnSimulatorIfHMD;
        }

        private void TurnOffSimulatorIfHMD(InputDevice device) => SetSimulatorEnabledIfHMD(device, false);
        private void TurnOnSimulatorIfHMD(InputDevice device) => SetSimulatorEnabledIfHMD(device, true);
        private void SetSimulatorEnabledIfHMD(InputDevice device, bool enable)
        {
            if (!device.characteristics.HasFlag(headsetCharacteristic))
                return;
            simulator.enabled = enable;
        }
    }
}