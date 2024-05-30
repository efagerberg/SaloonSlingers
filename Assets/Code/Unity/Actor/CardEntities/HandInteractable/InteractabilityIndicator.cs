using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// Tries to mirror some of the logic that the socket interactor does to communicate
    /// when something is interactable.
    /// General idea is to assist DeckGraphic interaction to communicate when a deck is
    /// drawable.
    /// </summary>
    public class InteractabilityIndicator : MonoBehaviour, IIndicator
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
        private BoxCollider scaleReference;

        public void Indicate(bool canInteract)
        {
            _renderer.enabled = true;
            var color = canInteract ? canInteractColor : cannotInteractColor;
            _renderer.material.color = color;
            var clip = canInteract ? canInteractClip : cannotInteractClip;

            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        public void Hide()
        {
            _renderer.enabled = false;
        }

        private void Awake()
        {
            var colliderSize = scaleReference.size;
            _renderer.transform.localScale = 1.001f * new Vector3(
                colliderSize.x / _renderer.transform.localScale.x,
                colliderSize.y / _renderer.transform.localScale.y,
                colliderSize.z / _renderer.transform.localScale.z
            );
            Hide();
        }
    }
}
