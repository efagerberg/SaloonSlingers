using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using GambitSimulator.Core;
using UnityEngine.XR.Interaction.Toolkit;

namespace GambitSimulator.Unity
{
    [RequireComponent(typeof(TrailRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ParticleSystem))]
    public class CardComponent : XRGrabInteractable
    {
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private float lifeTime = 1f;
        [SerializeField]
        private List<Renderer> cardRenderers;
        [SerializeField]
        private int maxAngularVelocity = 100;
        [SerializeField]
        private int spinFactor = 25;
        [SerializeField]
        private float sizeMultiplier = 2f;

        private Rigidbody rigidBody;
        private ParticleSystem destroyEffect;
        private TrailRenderer trailRenderer;

        public void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            rigidBody.isKinematic = true;
            destroyEffect = gameObject.GetComponent<ParticleSystem>();
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
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
            StartCoroutine(ThrowEffect());
        }

        public IEnumerator ThrowEffect()
        {
            rigidBody.isKinematic = false;
            rigidBody.AddTorque(transform.forward * rigidBody.velocity.magnitude * spinFactor);
            trailRenderer.enabled = true;
            yield return new WaitForSeconds(lifeTime);
            HideCard();
            rigidBody.isKinematic = true;
            trailRenderer.enabled = false;
            destroyEffect.Play();
            yield return new WaitForSeconds(destroyEffect.main.duration);
            destroyEffect.Stop();
        }

        public void HideCard()
        {
            foreach (var r in cardRenderers)
                r.enabled = false;
        }

        public void ShowCard()
        {
            foreach (var r in cardRenderers)
                r.enabled = true;
        }

        private string GetTexturePath(Card card)
        {
            return string.Format("Textures/{0}", card.ToString());
        }
    }
}
