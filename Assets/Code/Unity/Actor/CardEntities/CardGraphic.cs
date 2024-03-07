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
                faceMaterialManager.SetTexture(card);
            }
        }

        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private Material faceMaterial;

        private CardFaceManager faceMaterialManager;

        public void ResetActor()
        {
            faceMaterialManager.SetColor(Color.white);
        }

        public void Kill()
        {
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }

        public void SetColor(Color color) => faceMaterialManager.SetColor(color);

        private void Awake()
        {
            faceMaterialManager ??= new(faceRenderer);
        }

        private void OnValidate()
        {
            faceRenderer.sharedMaterial ??= faceMaterial;
            faceMaterialManager ??= new(faceRenderer);
            faceMaterialManager.SetTexture(card);
        }
    }
}
