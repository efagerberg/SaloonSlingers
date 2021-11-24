using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using GambitSimulator.Core;
using System;

namespace GambitSimulator.Unity
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
        private float sizeMultiplier = 2f;
        [SerializeField]
        private float maxLifetime = 2f;
        [SerializeField]
        private TrailRenderer trailRenderer;

        private Rigidbody rigidBody;
        private float timeToLive;

        public void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            rigidBody.isKinematic = true;
            timeToLive = maxLifetime;
            trailRenderer.enabled = false;
            transform.localScale = sizeMultiplier * new Vector3(transform.localScale.x, transform.localScale.y, 1);
        }

        public Card GetCard() => card;

        public void SetCard(Card inCard)
        {
            card = inCard;
            name = card.ToString();
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        protected override void Detach()
        {
            base.Detach();
            rigidBody.isKinematic = false;
            rigidBody.AddTorque(rigidBody.velocity.magnitude * spinFactor * transform.forward);
            trailRenderer.enabled = true;
        }

        private string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());

        private void FixedUpdate()
        {
            if (!isSelected)
            {
                if (rigidBody.velocity.magnitude == 0 || timeToLive <= 0)
                    DeactivateCard();
                else timeToLive -= Time.deltaTime;
            }
        }

        private void DeactivateCard()
        {
            timeToLive = maxLifetime;
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            gameObject.SetActive(false);
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            OnCardDeactivated?.Invoke(this, EventArgs.Empty);
        }
    }
}
