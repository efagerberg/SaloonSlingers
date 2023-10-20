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
        private GameObject gazeUI;

        private TemporaryHitPoints tempHitPoints;

        private void Start()
        {
            tempHitPoints = LevelManager.Instance.Player.GetComponent<TemporaryHitPoints>();
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

            absorber.Absorb(tempHitPoints, projectile);
        }

        public void OnSelectExit(SelectExitEventArgs _)
        {
            absorber.Cancel();
        }

        public void OnGazeEnter()
        {
            gazeUI.SetActive(true);
        }

        public void OnGazeExit()
        {
            gazeUI.SetActive(false);
        }
    }
}
