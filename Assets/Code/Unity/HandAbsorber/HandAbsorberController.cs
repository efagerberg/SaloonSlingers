using SaloonSlingers.Core;
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
        private XRBaseInteractor interactor;
        [SerializeField]
        private AudioClip errorClip;
        [Range(0, 1)]
        [SerializeField]
        private float volumeScale;

        private Attribute shieldHitPoints;
        private XRBaseInteractable plugInteractable;

        private void OnEnable()
        {
            if (absorber == null) return;

            absorber.Stacks.Increased += CheckIfShouldPlug;
            absorber.Stacks.Decreased += CheckIfShouldPlug;
        }

        private void OnDisable()
        {
            if (absorber == null) return;

            absorber.Stacks.Increased -= CheckIfShouldPlug;
            absorber.Stacks.Decreased -= CheckIfShouldPlug;
        }

        private void Start()
        {
            var origin = LevelManager.Instance.Player.GetComponent<XROrigin>();
            shieldHitPoints = origin.Camera.transform.parent.GetComponentInChildren<HoloShieldController>().HitPoints;

            var plugInstance = new GameObject("Hand Absorber Plug");
            plugInteractable = plugInstance.AddComponent<XRSimpleInteractable>();
            plugInteractable.interactionLayers = InteractionLayerMask.GetMask("HandInteractable");
            plugInteractable.transform.gameObject.SetActive(false);
            plugInteractable.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<CardHand>(out var projectile))
                return;

            if (!absorber.CanAbsorb && projectile.TryGetComponent<AudioSource>(out var audioSource))
                audioSource.PlayOneShot(errorClip, volumeScale);

            projectile.gameObject.GetComponentInImmediateChildren<HandLayout>().Stack();
        }

        public void OnHoverExit(HoverExitEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<CardHand>(out var projectile))
                return;

            projectile.gameObject.GetComponentInImmediateChildren<HandLayout>().Unstack();
        }

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<CardHand>(out var projectile))
                return;

            absorber.Absorb(shieldHitPoints, projectile);
        }

        private void CheckIfShouldPlug(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            if (e.After > 0)
            {
                if (interactor.IsSelecting(plugInteractable))
                {
                    plugInteractable.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    plugInteractable.gameObject.SetActive(false);
                }
                return;
            };

            plugInteractable.gameObject.SetActive(true);
            interactor.StartManualInteraction((IXRSelectInteractable)plugInteractable);
        }
    }
}
