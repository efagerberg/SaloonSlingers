using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{
    public class DashLocomotionProvider : LocomotionProvider
    {
        [SerializeField]
        private List<InputActionProperty> dashInputProperties;
        [SerializeField]
        private Dashable dashable;
        [SerializeField]
        private Transform forwardReference;

        private CharacterController characterController;

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
            if (dashable == null) dashable = system.xrOrigin.Origin.GetComponent<Dashable>();
            if (!CanBeginLocomotion()) return;

            BeginLocomotion();
            dashable.Dash(Move, forwardReference.forward);
            EndLocomotion();
        }

        private void Move(Vector3 v)
        {
            if (characterController == null) characterController = system.xrOrigin.Origin.GetComponent<CharacterController>();
            characterController.Move(v);
        }

        private void Update()
        {
            if (dashable == null) return;

            switch (locomotionPhase)
            {
                case LocomotionPhase.Idle:
                case LocomotionPhase.Started:
                    if (dashable.IsPerforming)
                        locomotionPhase = LocomotionPhase.Moving;
                    break;
                case LocomotionPhase.Moving:
                    if (!dashable.IsPerforming)
                        locomotionPhase = LocomotionPhase.Done;
                    break;
                case LocomotionPhase.Done:
                    locomotionPhase = dashable.IsPerforming ? LocomotionPhase.Moving : LocomotionPhase.Idle;
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(LocomotionPhase)}={locomotionPhase}");
                    break;
            }
        }
    }
}
