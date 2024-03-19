using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{
    public class DashLocomotionProvider : LocomotionProvider
    {
        [SerializeField]
        private List<InputActionProperty> dashInputProperties;
        [SerializeField]
        private CharacterController controller;
        [SerializeField]
        private Dashable dashable;
        [SerializeField]
        private Transform forwardReference;

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
            if (dashable == null) dashable = controller.GetComponent<Dashable>();
            if (!CanBeginLocomotion()) return;

            BeginLocomotion();
            dashable.Dash(Move, forwardReference.forward);
            EndLocomotion();
        }

        private void Move(Vector3 v) => controller.Move(v);
    }
}
