using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{
    public class DeckInteractabilityController : MonoBehaviour
    {
        [SerializeField]
        private InteractabilityIndicator indicator;

        public void OnHoverEntered(HoverEnterEventArgs args)
        {
            var player = LevelManager.Instance.Player;
            var attributeRegistry = player.GetComponent<Attributes>().Registry;
            ActorHandedness handedness = player.GetComponent<ActorHandedness>();
            indicator.transform.SetPositionAndRotation(handedness.DeckGraphic.TopCardTransform.position, handedness.DeckGraphic.TopCardTransform.rotation);
            DrawContext ctx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = handedness.DeckGraphic.Deck,
                Evaluation = HandEvaluation.EMPTY,
                Hand = HandInteractablePlacer.EMPTY_HAND
            };
            var canInteract = GameManager.Instance.Saloon.HouseGame.CanDraw(ctx);
            indicator.Indicate(canInteract);
        }

        public void OnHoverExited(HoverExitEventArgs args)
        {
            indicator.Hide();
        }
    }
}
