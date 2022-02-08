using UnityEngine;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class TangibleCard : MonoBehaviour
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
                CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
            }
        }
    }
}
