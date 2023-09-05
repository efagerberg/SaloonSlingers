using SaloonSlingers.Unity.CardEntities;

using UnityEngine;

namespace SaloonSlingers.Unity.Slingers
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
        private Transform enemyPeerPanelAttachTransform;
        [SerializeField]
        private GameObject enemyPeerPanelPrefab;

        private Transform deckAttachTransform;
        private bool isPrimary = false;

        private void Start()
        {
            ActorHandedness handedness = GetComponentInParent<ActorHandedness>();
            switch (handedness.Current)
            {
                case Handedness.RIGHT:
                    deckAttachTransform = leftDeckAttachTransform;
                    break;
                case Handedness.LEFT:
                    deckAttachTransform = rightDeckAttachTransform;
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
            else
            {
                GameObject clone = Instantiate(enemyPeerPanelPrefab, enemyPeerPanelAttachTransform);
                handedness.EnemyPeerDisplay = clone.GetComponent<EnemyHandDisplay>();
            }
        }
    }
}
