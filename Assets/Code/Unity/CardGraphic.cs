using UnityEngine;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class CardGraphic : MonoBehaviour, ICardGraphic
    {
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;

        public Card Card
        {
            get => card;
            set
            {
                card = value;
                name = card.ToString();
                if (faceRenderer == null) faceRenderer = GetComponent<Renderer>();
                SetGraphics(card);
            }
        }

        public void SetGraphics(Card card)
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        private static string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());
    }
}
