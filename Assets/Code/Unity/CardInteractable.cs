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

        public Card GetCard() => card;

        public void SetCard(Card inCard)
        {
            card = inCard;
            name = card.ToString();
            CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
        }

        protected override void Awake()
        {
            base.Awake();
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
        }

        public void DeactivateCard()
        {
            if (!gameObject.activeSelf) return;
            timeToLive = maxLifetime;
            SetUnthrownState();
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.SetActive(false);
            OnCardDeactivated?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            attachTransform = GetAttachTransformForInteractor(args.interactorObject);
            SetUnthrownState();
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            SetThrownState();
            rigidBody.AddTorque(spinFactor * transform.forward);
            NegateCharacterControllerVelocity(args.interactorObject);
        }

        private void NegateCharacterControllerVelocity(IXRInteractor interactorObject)
        {
            var c = interactorObject.transform.GetComponentInParent<CharacterController>();
            rigidBody.AddForce(-c.velocity);
        }

        private void FixedUpdate()
        {
            if (isSelected) return;

            if (rigidBody.velocity.magnitude == 0 || timeToLive <= 0)
                DeactivateCard();
            else timeToLive -= Time.deltaTime;
        }

        private Transform GetAttachTransformForInteractor(IXRInteractor interactor)
        {
            bool isRightHandInteractor = interactor.transform.name.Contains("right", StringComparison.OrdinalIgnoreCase);
            if (isRightHandInteractor) return rightAttachTransform;

            bool isLeftHandInteractor = interactor.transform.name.Contains("left", StringComparison.OrdinalIgnoreCase);
            if (isLeftHandInteractor) return leftAttachTransform;

            return rightAttachTransform;
        }

        private void SetThrownState()
        {
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
        }

        private void SetUnthrownState()
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            timeToLive = maxLifetime;
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (!hit.gameObject.CompareTag("Enemy")) return;

            var enemy = hit.GetComponent<Enemy>();
            Card inCard = enemy.GetCard();
            int remainingValue = Mathf.Max(inCard.Value - card.Value, 0);
            if (remainingValue == 0) Destroy(hit.gameObject);
            else
            {
                Card newCard = new Card(card.Suit, (Values)remainingValue);
                enemy.SetCard(newCard);
            }
            DeactivateCard();
        }
    }
}
