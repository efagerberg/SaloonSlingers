using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor {
    public class PlayerHandInteractabilityIndicator : MonoBehaviour
    {
        [SerializeField]
        private Renderer interactabilityRenderer;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private Color canInteractColor;
        [SerializeField]
        private Color cannotInteractColor;
        [SerializeField]
        private AudioClip canInteractClip;
        [SerializeField]
        private AudioClip cannotInteractClip;
        [SerializeField]
        private GameObject scaleReference;

        public void Check()
        {
            var player = LevelManager.Instance.Player;
            var attributeRegistry = player.GetComponent<Attributes>().Registry;
            ActorHandedness handedness = player.GetComponent<ActorHandedness>();
            DrawContext ctx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = handedness.DeckGraphic.Deck,
                Evaluation = HandEvaluation.EMPTY,
                Hand = new Card[] { }
            };
            var canInteract = GameManager.Instance.Saloon.HouseGame.CanDraw(ctx);
            interactabilityRenderer.transform.localScale = scaleReference.transform.localScale * 1.00001f;
            var color = canInteract ? canInteractColor : cannotInteractColor;
            interactabilityRenderer.material.color = color;
            var clip = canInteract ? canInteractClip : cannotInteractClip;

            if (clip != null)
                audioSource.PlayOneShot(clip);
            interactabilityRenderer.enabled = true;
        }

        public void Hide()
        {
            interactabilityRenderer.enabled = false;
        }
    }
}
