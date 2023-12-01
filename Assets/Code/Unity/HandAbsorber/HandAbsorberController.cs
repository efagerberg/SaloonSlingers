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
        [SerializeField]
        private CanvasGroup gazeUI;
        [SerializeField]
        private float fadeDuration = 0.5f;

        private HitPoints shieldHitPoints;
        private Transform gazer;
        private Coroutine absorbCoroutine;

        private void Start()
        {
            var origin = LevelManager.Instance.Player.GetComponent<XROrigin>();
            shieldHitPoints = origin.Camera.transform.parent.GetComponentInChildren<HitPoints>();
        }

        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            gazer = args.interactorObject.transform;

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

        public void OnGazeEnter(HoverEnterEventArgs args)
        {
            gazer = args.interactorObject.transform;

            if (!gameObject.activeInHierarchy) return;

            StartCoroutine(Fader.FadeTo(gazeUI, 1, fadeDuration));
        }

        public void OnGazeExit()
        {
            gazer = null;

            if (!gameObject.activeInHierarchy) return;

            StartCoroutine(Fader.FadeTo(gazeUI, 0, fadeDuration));
        }

        private void Update()
        {
            if (gazer == null) return;

            gazeUI.transform.forward = gazer.forward;
            gazeUI.transform.right = gazer.right;
        }
    }
}
