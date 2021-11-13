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
        private float lifeTime = 2f;
        [SerializeField]
        private List<Renderer> cardRenderers;
        private Rigidbody rigidBody;
        private ParticleSystem destroyEffect;
        private TrailRenderer trailRenderer;

        public void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            destroyEffect = gameObject.GetComponent<ParticleSystem>();
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
            trailRenderer.enabled = false;
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

        public void SetGraphics()
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        protected override void Detach()
        {
            base.Detach();
            rigidBody.isKinematic = false;
            trailRenderer.enabled = true;
            StartCoroutine(Burn());
        }

        public IEnumerator Burn()
        {
            yield return new WaitForSeconds(lifeTime);
            var childRenderers = transform.GetComponentsInChildren<Renderer>();
            foreach (var r in cardRenderers)
                r.enabled = false;
            rigidBody.isKinematic = true;
            destroyEffect.Play();
            yield return new WaitForSeconds(destroyEffect.main.duration);
            destroyEffect.Stop();
            foreach (var r in cardRenderers)
                r.enabled = true;
            trailRenderer.enabled = false;
            gameObject.SetActive(false);
        }

        private string GetTexturePath(Card card)
        {
            return string.Format("Textures/{0}", card.ToString());
        }
    }
}
