using SaloonSlingers.Unity.Actor;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class HandAbsorberController : MonoBehaviour
    {
        [SerializeField]
        private HandAbsorber absorber;

        private HitPoints shieldHitPoints;
        private Coroutine absorbCoroutine;

        private void Start()
        {
            var origin = LevelManager.Instance.Player.GetComponent<XROrigin>();
            shieldHitPoints = origin.Camera.transform.parent.GetComponentInChildren<HitPoints>();
        }

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

            absorbCoroutine = StartCoroutine(absorber.Absorb(shieldHitPoints, projectile));
        }

        public void OnSelectExit(SelectExitEventArgs _)
        {
            if (absorbCoroutine == null) return;

            StopCoroutine(absorbCoroutine);
        }
    }
}
