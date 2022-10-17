using UnityEngine;

using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.Slingers
{
    public class Hand : MonoBehaviour
    {
        [SerializeField]
        private Transform LeftDeckAttachTransform;
        [SerializeField]
        private Transform RightDeckAttachTransform;
        [SerializeField]
        private GameObject deckGraphicPrefab;

        private Transform deckAttachTransform;
        private bool isPrimary = false;
        private ISlinger slinger;

        private void Start()
        {
            slinger = GetComponentInParent<ISlinger>();
            switch (slinger.Attributes.Handedness)
            {
                case Handedness.RIGHT:
                    deckAttachTransform = LeftDeckAttachTransform;
                    break;
                case Handedness.LEFT:
                    deckAttachTransform = RightDeckAttachTransform;
                    break;
            }

            isPrimary = transform.parent.name.Contains(slinger.Attributes.Handedness.ToString(), System.StringComparison.CurrentCultureIgnoreCase);
            if (!isPrimary && deckAttachTransform != null) Instantiate(deckGraphicPrefab, deckAttachTransform);
        }
    }
}
