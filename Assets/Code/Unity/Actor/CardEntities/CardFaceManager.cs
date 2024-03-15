using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class CardFaceManager
    {
        private readonly Material faceMaterial;
        private readonly Renderer faceRenderer;

        public CardFaceManager(Renderer faceRenderer)
        {
            this.faceRenderer = faceRenderer;
            // In order to avoid editor errors around instantiating a
            // material implicitly
            faceMaterial = new(this.faceRenderer.sharedMaterial);
            this.faceRenderer.material = faceMaterial;
        }

        public void SetTexture(Card card)
        {
            faceRenderer.material = faceMaterial;
            faceMaterial.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        public void SetColor(Color color)
        {
            faceMaterial.color = color;
        }

        private static string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());
    }
}
