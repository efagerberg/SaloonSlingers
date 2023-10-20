using SaloonSlingers.Unity.Actor;

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

        private TemporaryHitPoints tempHitPoints;
        private Transform gazer;

        private void Start()
        {
            tempHitPoints = LevelManager.Instance.Player.GetComponent<TemporaryHitPoints>();
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

            absorber.Absorb(tempHitPoints, projectile);
        }

        public void OnSelectExit(SelectExitEventArgs _)
        {
            absorber.Cancel();
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

            gazeUI.transform.LookAt(-gazer.position);
        }
    }
}
