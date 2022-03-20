using UnityEngine;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class TangibleCard : MonoBehaviour, ITangibleCard
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
                CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
            }
        }
    }
}
