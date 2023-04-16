using UnityEngine;

namespace SaloonSlingers.Unity.Slingers
{
    public class SlingerArmGraphic : MonoBehaviour
    {
        [SerializeField]
        private Transform LeftDeckAttachTransform;
        [SerializeField]
        private Transform RightDeckAttachTransform;
        [SerializeField]
        private GameObject deckGraphicPrefab;

        private Transform deckAttachTransform;
        private bool isPrimary = false;

        private void Start()
        {
            SlingerHandedness handedness = GetComponentInParent<SlingerHandedness>();
            switch (handedness.Current)
            {
                case Handedness.RIGHT:
                    deckAttachTransform = LeftDeckAttachTransform;
                    break;
                case Handedness.LEFT:
                    deckAttachTransform = RightDeckAttachTransform;
                    break;
            }

            isPrimary = transform.parent.name.Contains(
                handedness.Current.ToString(),
                System.StringComparison.CurrentCultureIgnoreCase
            );
            if (!isPrimary && deckAttachTransform != null)
                Instantiate(deckGraphicPrefab, deckAttachTransform);
        }
    }
}
