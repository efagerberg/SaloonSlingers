using System;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public delegate void CardDeactivatedEventHandler(CardInteractable sender, EventArgs e);

    public class CardInteractable : XRGrabInteractable
    {
        public static event CardDeactivatedEventHandler OnCardDeactivated;

        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private int maxAngularVelocity = 100;
        [SerializeField]
        private int spinFactor = 25;
        [SerializeField]
        private float maxLifetime = 2f;
        [SerializeField]
        private TrailRenderer trailRenderer;
        [SerializeField]
        private Transform leftAttachTransform;
        [SerializeField]
        private Transform rightAttachTransform;

        private Rigidbody rigidBody;
        private float timeToLive;

        public void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            rigidBody.isKinematic = true;
            timeToLive = maxLifetime;
            trailRenderer.enabled = false;
        }

        public Card GetCard() => card;

        public void SetCard(Card inCard)
        {
            card = inCard;
            name = card.ToString();
            CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
        }

        protected override void Detach()
        {
            base.Detach();
            rigidBody.isKinematic = false;
            rigidBody.AddTorque(rigidBody.velocity.magnitude * spinFactor * transform.forward);
            trailRenderer.enabled = true;
        }

        private void FixedUpdate()
        {
            if (isSelected) return;

            if (rigidBody.velocity.magnitude == 0 || timeToLive <= 0)
                DeactivateCard();
            else timeToLive -= Time.deltaTime;
        }

        public void DeactivateCard()
        {
            if (!gameObject.activeSelf) return;
            timeToLive = maxLifetime;
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.SetActive(false);
            OnCardDeactivated?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            bool isRightHandInteractor = args.interactor.name.Contains("right", StringComparison.OrdinalIgnoreCase);
            if (isRightHandInteractor) attachTransform = rightAttachTransform;

            bool isLeftHandInteractor = args.interactor.name.Contains("left", StringComparison.OrdinalIgnoreCase);
            if (isLeftHandInteractor) attachTransform = leftAttachTransform;
        }
    }
}
