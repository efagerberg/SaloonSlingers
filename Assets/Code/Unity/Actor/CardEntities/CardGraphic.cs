using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CardGraphic : MonoBehaviour, ICardGraphic
    {
        public Material FaceMaterial { get => faceRenderer.material; }

        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;

        public event EventHandler Death;

        public Card Card
        {
            get => card;
            set
            {
                card = value;
                name = card.ToUnicode();
                if (faceRenderer == null) faceRenderer = GetComponent<Renderer>();
                SetGraphics(card);
            }
        }

        public void SetGraphics(Card card)
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        public void Reset()
        {
            faceRenderer.material.mainTexture = null;
        }

        public void Kill()
        {
            Death?.Invoke(gameObject, EventArgs.Empty);
        }

        private static string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());
    }
}
