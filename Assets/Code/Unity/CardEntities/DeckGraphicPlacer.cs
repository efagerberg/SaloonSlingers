using SaloonSlingers.Unity.CardEntities;

using UnityEngine;

namespace SaloonSlingers.Unity.Slingers
{
    public class DeckGraphicPlacer : MonoBehaviour
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

            isPrimary = transform.parent.parent.name.Contains(
                handedness.Current.ToString(),
                System.StringComparison.CurrentCultureIgnoreCase
            );
            if (!isPrimary)
            {
                GameObject clone = Instantiate(deckGraphicPrefab, deckAttachTransform);
                handedness.DeckGraphic = clone.GetComponent<DeckGraphic>();
            }
        }
    }
}
