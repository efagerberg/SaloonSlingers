using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class HandAbsorberController : MonoBehaviour
    {
        [SerializeField]
        private HandAbsorber absorber;

        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<HandProjectile>(out var projectile)) return;

            projectile.Stack();
        }

        public void OnHoverExit(HoverExitEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<HandProjectile>(out var projectile)) return;

            projectile.Unstack();
        }

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<HandProjectile>(out var projectile)) return;

            absorber.Absorb(projectile);
        }

        public void OnSelectExit(SelectExitEventArgs _)
        {
            absorber.Cancel();
        }
    }
}
