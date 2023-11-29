using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HandInitializer : MonoBehaviour
    {
        [SerializeField]
        private Transform leftDeckAttachTransform;
        [SerializeField]
        private Transform rightDeckAttachTransform;
        [SerializeField]
        private GameObject deckGraphicPrefab;
        [SerializeField]
        private Transform leftAbsorberAttachTransform;
        [SerializeField]
        private Transform rightAbsorberAttachTransform;
        [SerializeField]
        private GameObject absorberPrefab;
        [SerializeField]
        private Transform enemyPeerPanelAttachTransform;
        [SerializeField]
        private GameObject enemyPeerPanelPrefab;
        [SerializeField]
        private Transform leftShieldAttachTransform;
        [SerializeField]
        private Transform rightShieldAttachTransform;
        [SerializeField]
        private GameObject shieldPrefab;

        private Transform deckAttachTransform;
        private Transform absorberAttachTransform;
        private Transform shieldAttachTransform;
        private bool isPrimary = false;

        private void Start()
        {
            ActorHandedness handedness = GetComponentInParent<ActorHandedness>();
            switch (handedness.Current)
            {
                case Handedness.RIGHT:
                    deckAttachTransform = leftDeckAttachTransform;
                    absorberAttachTransform = leftAbsorberAttachTransform;
                    shieldAttachTransform = leftShieldAttachTransform;
                    break;
                case Handedness.LEFT:
                    deckAttachTransform = rightDeckAttachTransform;
                    absorberAttachTransform = rightAbsorberAttachTransform;
                    shieldAttachTransform = rightShieldAttachTransform;
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
                Instantiate(absorberPrefab, absorberAttachTransform);
                Instantiate(shieldPrefab, shieldAttachTransform);
            }
            else
            {
                GameObject clone = Instantiate(enemyPeerPanelPrefab, enemyPeerPanelAttachTransform);
                handedness.EnemyPeerDisplay = clone.GetComponent<EnemyHandDisplay>();
            }
        }
    }
}
