using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Slingers
{
    public class DashProvider : LocomotionProvider
    {
        [SerializeField]
        private List<InputActionProperty> dashInputProperties;

        private CharacterController controller;
        private Dashable dashable;

        private void Start()
        {
            controller = system.xrOrigin.GetComponent<CharacterController>();
            dashable = system.xrOrigin.GetComponent<Dashable>();
        }

        private void OnEnable()
        {
            foreach (var property in dashInputProperties)
                property.action.performed += HandleDash;
        }

        private void OnDisable()
        {
            foreach (var property in dashInputProperties)
                property.action.performed -= HandleDash;
        }

        private void HandleDash(InputAction.CallbackContext context)
        {
            dashable.Dash(controller, system.xrOrigin.Camera.transform.forward);
        }
    }
}
