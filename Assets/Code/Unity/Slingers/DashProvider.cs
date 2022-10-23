using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.Slingers
{
    public class DashProvider : LocomotionProvider
    {
        [SerializeField]
        private List<InputActionProperty> dashInputProperties;

        private bool canDash = true;
        private Dash dash;
        private CharacterController controller;

        private void Start()
        {
            controller = system.xrOrigin.GetComponent<CharacterController>();
            PlayerAttributes attributes = (PlayerAttributes)system.xrOrigin.GetComponent<Player>().Attributes;
            dash = attributes.Dash;
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
            canDash = false;
            dash.DashPoints.Value -= 1;
            float currentDuration = dash.Duration;
            while (currentDuration > 0)
            {
                controller.Move(dash.Speed * Time.deltaTime * system.xrOrigin.Camera.transform.forward);
                currentDuration -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(dash.CoolDown);
            canDash = dash.DashPoints.Value > 0;

            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them.
            yield return new WaitForSeconds(dash.PointRecoveryPeriod - dash.CoolDown);
            dash.DashPoints.Value += 1;
            canDash = true;
        }
    }
}
