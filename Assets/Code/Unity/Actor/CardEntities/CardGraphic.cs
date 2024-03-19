using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CardGraphic : MonoBehaviour, ICardGraphic
    {
        public event EventHandler Killed;

        public Card Card
        {
            get => card;
            set
            {
                card = value;
                name = card.ToUnicode();
                faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
            }
        }

        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private Material faceMaterial;

        public void ResetActor()
        {
            SetColor(Color.white);
        }

        public void Kill()
        {
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }

        public void SetColor(Color color) => faceRenderer.material.color = color;

        public void SetTexture(Card card)
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        private static string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());
    }
}
