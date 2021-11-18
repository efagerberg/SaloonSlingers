using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using GambitSimulator.Core;
using UnityEngine.XR.Interaction.Toolkit;

namespace GambitSimulator.Unity
{
    public class CardInteractable : XRGrabInteractable
    {
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private List<Renderer> cardRenderers;
        [SerializeField]
        private int maxAngularVelocity = 100;
        [SerializeField]
        private int spinFactor = 25;
        [SerializeField]
        private float sizeMultiplier = 2f;
        [SerializeField]
        private TrailRenderer trailRenderer;

        private Rigidbody rigidBody;

        public void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            rigidBody.isKinematic = true;
            trailRenderer.enabled = false;
            transform.localScale = sizeMultiplier * new Vector3(transform.localScale.x, transform.localScale.y, 1);
        }

        public Card GetCard()
        {
            return card;
        }

        public void SetCard(Card inCard)
        {
            card = inCard;
            name = card.ToString();
            SetGraphics();
        }

        private void SetGraphics()
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        protected override void Detach()
        {
            base.Detach();
            rigidBody.isKinematic = false;
            rigidBody.AddTorque(transform.forward * rigidBody.velocity.magnitude * spinFactor);
            trailRenderer.enabled = true;
        }

        private string GetTexturePath(Card card)
        {
            return string.Format("Textures/{0}", card.ToString());
        }

        private void FixedUpdate()
        {
            if (!isSelected && rigidBody.velocity.magnitude == 0)
                Destroy(gameObject);
        }
    }
}
