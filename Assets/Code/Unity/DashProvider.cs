using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

using SaloonSlingers.Core;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class DashProvider : LocomotionProvider
    {
        [SerializeField]
        private InputActionManager inputManager;
        [SerializeField]
        private float dashDuration = 0.25f;
        [SerializeField]
        private List<InputActionProperty> dashInputProperties;

        private bool canDash = true;
        private PlayerAttributes attributes;
        private CharacterController controller;

        private void Start()
        {
            controller = system.xrOrigin.GetComponent<CharacterController>();
            attributes = system.xrOrigin.GetComponent<Player>().Attributes;
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
            if (!canDash) return;
            StartCoroutine(nameof(Dash));
        }

        private IEnumerator Dash()
        {
            inputManager.DisableInput();
            canDash = false;
            attributes.Dashes -= 1;
            var originalDashTime = dashDuration;
            while (dashDuration > 0)
            {
                controller.Move(attributes.DashSpeed * Time.deltaTime * system.xrOrigin.Camera.transform.forward);
                dashDuration -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            dashDuration = originalDashTime;

            if (attributes.Dashes > 0) canDash = true;
            inputManager.EnableInput();

            yield return new WaitForSeconds(attributes.DashCooldown);
            attributes.Dashes += 1;
            canDash = true;
        }
    }
}
