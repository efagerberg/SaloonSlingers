using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CardGraphic : Actor, ICardGraphic
    {
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

        public Color Color { get => faceRenderer.material.color; set => faceRenderer.material.color = value; }

        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private Material faceMaterial;

        public override void ResetActor()
        {
            Color = Color.white;
        }

        public void Kill()
        {
            OnKilled.Invoke(gameObject);
        }

        public void SetTexture(Card card)
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        private static string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());
    }
}
