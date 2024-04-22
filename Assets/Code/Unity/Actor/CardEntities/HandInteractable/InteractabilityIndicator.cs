using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// Tries to mirror some of the logic that the socket interactor does to communicate
    /// when something is interactable.
    /// General idea is to assist DeckGraphic interaction to communicate when a deck is
    /// drawable.
    /// </summary>
    public class InteractabilityIndicator : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;
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

        public void Indicate(bool canInteract)
        {
            _renderer.transform.localScale = scaleReference.transform.localScale;
            var color = canInteract ? canInteractColor : cannotInteractColor;
            _renderer.material.color = color;
            var clip = canInteract ? canInteractClip : cannotInteractClip;

            if (clip != null)
                audioSource.PlayOneShot(clip);
            _renderer.enabled = true;
        }

        public void Hide()
        {
            _renderer.enabled = false;
        }
    }
}
